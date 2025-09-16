using System.ComponentModel.DataAnnotations;

namespace FileNet.WebFramework.Contracts.Common;

public class PageRequest
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 200;

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = DefaultPage;

    [Range(1, MaxPageSize)]
    public int PageSize { get; set; } = DefaultPageSize;

    public string? SortBy { get; set; }

    public bool Desc { get; set; }

    public string? Search { get; set; }

    public int Skip => Math.Max(0, (NormalizePage(Page) - 1) * NormalizePageSize(PageSize));

    public static int NormalizePage(int page) => page <= 0 ? DefaultPage : page;

    public static int NormalizePageSize(int pageSize)
        => pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);

    public void Normalize()
    {
        Page = NormalizePage(Page);
        PageSize = NormalizePageSize(PageSize);
        if (string.IsNullOrWhiteSpace(SortBy)) SortBy = null;
        if (string.IsNullOrWhiteSpace(Search)) Search = null;
    }
}
