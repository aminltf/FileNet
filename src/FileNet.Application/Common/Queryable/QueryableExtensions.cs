using System.Linq.Dynamic.Core;

namespace FileNet.Application.Common.Queryable;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        SortOptions sortOptions)
    {
        if (sortOptions?.Fields == null || sortOptions.Fields.Count == 0)
            return query;

        var direction = sortOptions.Direction == SortDirection.Asc ? "ascending" : "descending";
        var sortExpression = string.Join(",", sortOptions.Fields.Select(f => $"{f} {direction}"));

        return query.OrderBy(sortExpression);
    }

    public static IQueryable<T> ApplyPaging<T>(
        this IQueryable<T> query,
        PageRequest request)
    {
        if (request == null || !request.IsPagingEnabled)
            return query;

        int pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
        int pageSize = request.PageSize > 0 ? request.PageSize : 10;

        const int maxPageSize = 10000;
        pageSize = Math.Min(pageSize, maxPageSize);

        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}
