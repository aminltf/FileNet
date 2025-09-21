using FileNet.WebFramework.Contracts.Common;
using FileNet.WebFramework.Contracts.Departments;
using FileNet.WebFramework.Contracts.Employees;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileNet.WebApplication.Pages.Departments;

public class EmployeesModel : PageModel
{
    private readonly IDepartmentService _deps;
    public EmployeesModel(IDepartmentService deps) => _deps = deps;

    [FromRoute(Name = "id")] public Guid DepartmentId { get; set; }

    [TempData] public string? Success { get; set; }
    [TempData] public string? Error { get; set; }

    [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;
    [BindProperty(SupportsGet = true)] public int PageSize { get; set; } = 10;
    [BindProperty(SupportsGet = true)] public string? SearchTerm { get; set; }
    [BindProperty(SupportsGet = true)] public string? SortColumn { get; set; }
    [BindProperty(SupportsGet = true)] public SortDirection SortDirection { get; set; } = SortDirection.Asc;

    public PageResponse<EmployeeDto>? Page { get; private set; }
    public IReadOnlyList<EmployeeDto> Items => Page?.Items;
    public DepartmentDto? Department { get; private set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        // normalize
        if (PageNumber <= 0) PageNumber = 1;
        if (PageSize <= 0) PageSize = 10;

        var request = new PagedRequest
        {
            PageNumber = PageNumber,
            PageSize = PageSize,
            SearchTerm = SearchTerm,
            SortColumn = SortColumn,
            SortDirection = SortDirection
        };

        Department = await _deps.GetByIdAsync(DepartmentId, ct);

        Page = await _deps.GetEmployeesPagedAsync(DepartmentId, request, ct);

        if (Page is not null)
        {
            PageNumber = Page.PageNumber;
            PageSize = Page.PageSize;
        }

        return Page();
    }
}
