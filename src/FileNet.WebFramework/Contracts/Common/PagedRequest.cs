namespace FileNet.WebFramework.Contracts.Common;

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

public enum SortDirection
{
    Asc = 0,
    Desc = 1
}
