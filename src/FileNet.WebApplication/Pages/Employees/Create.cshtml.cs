using FileNet.WebFramework.Contracts.Employees;
using FileNet.WebFramework.Services.Abstractions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FileNet.WebFramework.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FileNet.WebApplication.Pages.Employees;

public class CreateModel(
    IEmployeeService employeeService,
    IDepartmentService departmentService) : PageModel
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IDepartmentService _departmentService = departmentService;

    [BindProperty] public CreateEmployeeInput Input { get; set; } = new();
    public List<SelectListItem> DepartmentOptions { get; private set; } = new();

    [TempData] public string? Error { get; set; }

    public async Task OnGetAsync(CancellationToken ct)
    {
        await LoadLookupsAsync(ct);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        await LoadLookupsAsync(ct);

        if (!ModelState.IsValid) return Page();

        try
        {
            var id = await _employeeService.CreateAsync(new EmployeeCreateDto
            {
                NationalCode = Input.NationalCode.Trim(),
                FirstName = Input.FirstName.Trim(),
                LastName = Input.LastName.Trim(),
                Gender = Input.Gender,
                DepartmentId = Input.DepartmentId
            }, ct);

            return RedirectToPage("Details", new { id });
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            return Page();
        }
    }

    private async Task LoadLookupsAsync(CancellationToken ct)
    {
        var items = await _departmentService.GetLookupAsync(ct);

        DepartmentOptions = items
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            .ToList();
    }

    public class CreateEmployeeInput
    {
        [Required, StringLength(10)]
        [Display(Name = "کدملی")]
        public string NationalCode { get; set; } = default!;

        [Required, StringLength(100)]
        [Display(Name = "نام")]
        public string FirstName { get; set; } = default!;

        [Required, StringLength(100)]
        [Display(Name = "نام‌خانوادگی")]
        public string LastName { get; set; } = default!;

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public Guid DepartmentId { get; set; }
    }

    public static IEnumerable<(byte Value, string Name)> Genders =>
        Enum.GetValues<Gender>().Select(e => ((byte)e, e.ToString()));
}
