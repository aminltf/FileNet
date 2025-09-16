namespace FileNet.WebFramework.Contracts.Common;

public class PageResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public string? SortBy { get; set; }

    public bool Desc { get; set; }

    public string? Search { get; set; }

    public int TotalPages => PageSize <= 0
        ? 0
        : (int)Math.Ceiling((double)TotalCount / PageSize);

    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
