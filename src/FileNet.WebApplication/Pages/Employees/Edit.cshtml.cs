using FileNet.WebFramework.Contracts.Employees;
using FileNet.WebFramework.Services.Abstractions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FileNet.WebFramework.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using FileNet.WebFramework.Services.Implementations;

namespace FileNet.WebApplication.Pages.Employees;

public class EditModel(IEmployeeService employeeService, IDepartmentService departmentService) : PageModel
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IDepartmentService _departmentService = departmentService;

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty] public EditEmployeeInput Input { get; set; } = new();
    public List<SelectListItem> DepartmentOptions { get; private set; } = new();

    [TempData] public string? Error { get; set; }
    [TempData] public string? Success { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        await LoadLookupsAsync(ct);

        var e = await _employeeService.GetByIdAsync(Id, ct);
        if (e is null) return NotFound();

        Input = new EditEmployeeInput
        {
            NationalCode = e.NationalCode,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Gender = e.Gender,
            DepartmentId = e.DepartmentId,
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            await _employeeService.UpdateAsync(new EmployeeUpdateDto
            {
                Id = Id,
                NationalCode = Input.NationalCode.Trim(),
                FirstName = Input.FirstName.Trim(),
                LastName = Input.LastName.Trim(),
                Gender = Input.Gender,
                DepartmentId = Input.DepartmentId,
            }, ct);

            Success = "Employee updated.";
            return RedirectToPage("Details", new { id = Id });
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

    public class EditEmployeeInput
    {
        [Required, StringLength(10)]
        [Display(Name = "National Code")]
        public string NationalCode { get; set; } = default!;

        [Required, StringLength(100)]
        public string FirstName { get; set; } = default!;

        [Required, StringLength(100)]
        public string LastName { get; set; } = default!;

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public Guid DepartmentId { get; set; }
    }

    public static IEnumerable<(byte Value, string Name)> Genders =>
        Enum.GetValues<Gender>().Select(e => ((byte)e, e.ToString()));
}
