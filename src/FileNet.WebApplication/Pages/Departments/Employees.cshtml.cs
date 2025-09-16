using FileNet.WebFramework.Contracts.Departments;
using FileNet.WebFramework.Contracts.Employees;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileNet.WebApplication.Pages.Departments;

public class EmployeesModel : PageModel
{
    private readonly IDepartmentService _deps;
    private readonly IEmployeeService _emps;

    public EmployeesModel(IDepartmentService deps, IEmployeeService emps)
    {
        _deps = deps;
        _emps = emps;
    }

    [FromRoute(Name = "id")]
    public Guid DepartmentId { get; set; }

    [TempData] public string? Success { get; set; }
    [TempData] public string? Error { get; set; }

    public DepartmentDto? Department { get; private set; }
    public IReadOnlyList<EmployeeDto> Items { get; private set; } = Array.Empty<EmployeeDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        if (DepartmentId == Guid.Empty) return NotFound();

        Department = await _deps.GetByIdAsync(DepartmentId, ct);
        if (Department is null) return NotFound();

        Items = await _deps.GetEmployeesAsync(DepartmentId, ct);
        return Page();
    }
}
