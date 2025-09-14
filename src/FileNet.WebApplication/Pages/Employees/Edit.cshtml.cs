using FileNet.WebFramework.Contracts.Employees;
using FileNet.WebFramework.Services.Abstractions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileNet.WebApplication.Pages.Employees;

public class EditModel(IEmployeeService service) : PageModel
{
    private readonly IEmployeeService _service = service;

    [FromRoute] public Guid Id { get; set; }

    [BindProperty] public EditEmployeeInput Input { get; set; } = new();

    [TempData] public string? Error { get; set; }
    [TempData] public string? Success { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var e = await _service.GetByIdAsync(Id, ct);
        if (e is null) return NotFound();

        Input = new EditEmployeeInput
        {
            NationalCode = e.NationalCode,
            FirstName = e.FirstName,
            LastName = e.LastName
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            await _service.UpdateAsync(new EmployeeUpdateDto
            {
                NationalCode = Input.NationalCode.Trim(),
                FirstName = Input.FirstName.Trim(),
                LastName = Input.LastName.Trim()
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

    public class EditEmployeeInput
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
