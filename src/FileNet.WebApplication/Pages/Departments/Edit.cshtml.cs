using System.ComponentModel.DataAnnotations;
using FileNet.WebFramework.Contracts.Departments;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileNet.WebApplication.Pages.Departments;

public class EditModel(IDepartmentService service) : PageModel
{
    private readonly IDepartmentService _service = service;

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty] public EditDepartmentInput Input { get; set; } = new();

    [TempData] public string? Error { get; set; }
    [TempData] public string? Success { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var d = await _service.GetByIdAsync(Id, ct);
        if (d is null) return NotFound();

        Input = new EditDepartmentInput
        {
            Code = d.Code,
            Name = d.Name,
            Description = d.Description
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
                Id = Id,
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
