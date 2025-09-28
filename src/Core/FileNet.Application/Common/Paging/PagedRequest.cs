namespace FileNet.Application.Common.Paging;

public enum SortDirection { Asc, Desc }

public class PagedRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; }
    public SortDirection SortDirection { get; set; } = SortDirection.Asc;
    public int Skip => (PageNumber - 1) * PageSize;
    public bool IsPagingEnabled => PageSize > 0;
}
