using System.Collections.Concurrent;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using FileNet.Application.Common.Paging;

namespace FileNet.Application.Common.Extensions;

public static class QueryableExtensions
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _stringPropsCache = new();

    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, PagedRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SortColumn))
            return query;

        var direction = request.SortDirection == SortDirection.Asc ? "ascending" : "descending";

        return query.OrderBy($"{request.SortColumn} {direction}");
    }

    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, PagedRequest request)
    {
        if (request == null || !request.IsPagingEnabled)
            return query;

        int pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
        int pageSize = request.PageSize > 0 ? request.PageSize : 10;

        const int maxPageSize = 10000;
        pageSize = Math.Min(pageSize, maxPageSize);

        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    public static IQueryable<T> ApplySearching<T>(this IQueryable<T> query, PagedRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.SearchTerm))
            return query;

        var term = request.SearchTerm.Trim();
        var props = _stringPropsCache.GetOrAdd(
            typeof(T),
            t => t.GetProperties()
                  .Where(p => p.PropertyType == typeof(string) && p.CanRead)
                  .ToArray());

        if (props.Length == 0) return query;

        var p = Expression.Parameter(typeof(T), "x");
        Expression? body = null;

        foreach (var prop in props)
        {
            var member = Expression.Property(p, prop);
            var nullChk = Expression.NotEqual(member, Expression.Constant(null, typeof(string)));
            var contains = Expression.Call(
                member,
                nameof(string.Contains),
                Type.EmptyTypes,
                Expression.Constant(term, typeof(string)));

            var expr = Expression.AndAlso(nullChk, contains);
            body = body is null ? expr : Expression.OrElse(body, expr);
        }

        var lambda = Expression.Lambda<Func<T, bool>>(body!, p);
        return query.Where(lambda);
    }
}
