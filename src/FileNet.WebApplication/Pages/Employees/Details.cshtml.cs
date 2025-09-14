using FileNet.WebFramework.Contracts.Employees;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileNet.WebApplication.Pages.Employees;

public class DetailsModel(IEmployeeService employeeService) : PageModel
{
    private readonly IEmployeeService _svc = employeeService;

    [FromRoute] public Guid Id { get; set; }
    public EmployeeDto? Item { get; private set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        Item = await _svc.GetByIdAsync(Id, ct);
        if (Item is null) return NotFound();
        return Page();
    }
}
