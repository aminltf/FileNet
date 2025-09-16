using System.ComponentModel.DataAnnotations;
using FileNet.WebFramework.Contracts.Departments;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileNet.WebApplication.Pages.Departments;

public class EditModel(IDepartmentService service) : PageModel
{
    private readonly IDepartmentService _service = service;

    [FromRoute] public Guid Id { get; set; }

    [BindProperty] public EditDepartmentInput Input { get; set; } = new();

    [TempData] public string? Error { get; set; }
    [TempData] public string? Success { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var e = await _service.GetByIdAsync(Id, ct);
        if (e is null) return NotFound();

        Input = new EditDepartmentInput
        {
            Code = e.Code,
            Name = e.Name,
            Description = e.Description
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            await _service.UpdateAsync(new DepartmentUpdateDto
            {
                Code = Input.Code.Trim(),
                Name = Input.Name.Trim(),
                Description = Input.Description?.Trim()
            }, ct);

            Success = "Department updated.";
            return RedirectToPage("Details", new { id = Id });
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            return Page();
        }
    }

    public class EditDepartmentInput
    {
        [Required]
        public string Code { get; set; } = default!;

        [Required]
        public string Name { get; set; } = default!;

        [StringLength(100)]
        public string? Description { get; set; }
    }
}
