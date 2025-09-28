using FileNet.WebFramework.Contracts.Common;
using FileNet.WebFramework.Contracts.Dependents;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileNet.WebApplication.Pages.Dependents;

public class IndexModel(IDependentService svc) : PageModel
{
    private readonly IDependentService _svc = svc;

    [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;
    [BindProperty(SupportsGet = true)] public int PageSize { get; set; } = 10;
    [BindProperty(SupportsGet = true)] public string? SearchTerm { get; set; }
    [BindProperty(SupportsGet = true)] public string? SortColumn { get; set; }
    [BindProperty(SupportsGet = true)] public SortDirection SortDirection { get; set; } = SortDirection.Asc;

    public PageResponse<DependentDto> Page { get; private set; }
    public IReadOnlyList<DependentDto> Items => Page?.Items;

    [TempData] public string? Success { get; set; }
    [TempData] public string? Error { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        // normalize fallbacks
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

        Page = await _svc.GetPagedAsync(request, ct);

        // sync back normalized values (service might clamp them)
        if (Page is not null)
        {
            PageNumber = Page.PageNumber;
            PageSize = Page.PageSize;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, CancellationToken ct)
    {
        try
        {
            await _svc.DeleteAsync(id, ct);
            Success = "Dependent deleted.";
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }

        // Preserve current query parameters after PRG
        return RedirectToPage(new
        {
            PageNumber,
            PageSize,
            SearchTerm,
            SortColumn,
            SortDirection
        });
    }
}
