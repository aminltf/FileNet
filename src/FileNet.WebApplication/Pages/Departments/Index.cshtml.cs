using FileNet.WebFramework.Contracts.Departments;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileNet.WebApplication.Pages.Departments;

public class IndexModel(IDepartmentService departmentService) : PageModel
{
    private readonly IDepartmentService _svc = departmentService;
    public IReadOnlyList<DepartmentDto> Items { get; private set; } = [];

    [TempData] public string? Success { get; set; }
    [TempData] public string? Error { get; set; }

    public async Task OnGetAsync(CancellationToken ct)
    {
        Items = await _svc.GetAllAsync(ct);
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, CancellationToken ct)
    {
        try
        {
            await _svc.DeleteAsync(id, ct);
            Success = "Department deleted.";
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        return RedirectToPage();
    }
}
