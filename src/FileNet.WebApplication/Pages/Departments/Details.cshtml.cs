using FileNet.WebFramework.Contracts.Departments;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileNet.WebApplication.Pages.Departments;

public class DetailsModel(IDepartmentService service) : PageModel
{
    private readonly IDepartmentService _service = service;

    [FromRoute] public Guid Id { get; set; }

    public DepartmentDto? Item { get; private set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        Item = await _service.GetByIdAsync(Id, ct);
        if (Item is null) return NotFound();
        return Page();
    }
}
