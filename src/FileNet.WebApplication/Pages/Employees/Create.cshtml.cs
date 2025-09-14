using FileNet.WebFramework.Contracts.Employees;
using FileNet.WebFramework.Services.Abstractions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileNet.WebApplication.Pages.Employees;

public class CreateModel(IEmployeeService service) : PageModel
{
    private readonly IEmployeeService _service = service;

    [BindProperty] public CreateEmployeeInput Input { get; set; } = new();

    [TempData] public string? Error { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            var id = await _service.CreateAsync(new EmployeeCreateDto
            {
                NationalCode = Input.NationalCode.Trim(),
                FirstName = Input.FirstName.Trim(),
                LastName = Input.LastName.Trim()
            }, ct);

            return RedirectToPage("Details", new { id });
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            return Page();
        }
    }

    public class CreateEmployeeInput
    {
        [Required, StringLength(10)]
        [Display(Name = "National Code")]
        public string NationalCode { get; set; } = default!;

        [Required, StringLength(100)]
        public string FirstName { get; set; } = default!;

        [Required, StringLength(100)]
        public string LastName { get; set; } = default!;
    }
}
