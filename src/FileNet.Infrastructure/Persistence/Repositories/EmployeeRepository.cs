using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Queryable;
using FileNet.Domain.Entities;
using FileNet.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FileNet.Infrastructure.Persistence.Repositories;

public class EmployeeRepository : EfRepository<Employee>, IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext db) : base(db)
    {
        _context = db;
    }

    public async Task<PageResponse<Employee>> GetPagedAsync(
        PageRequest page,
        SearchRequest search,
        SortOptions sort,
        CancellationToken ct)
    {
        var dep = _context.Employees
            .AsQueryable()
            .AsNoTracking();

        // Search
        if (!string.IsNullOrWhiteSpace(search?.SearchTerm))
        {
            var term = $"%{search.SearchTerm.Trim()}%";
            dep = dep.Where(x =>
                EF.Functions.Like(x.NationalCode, term) ||
                EF.Functions.Like(x.FirstName, term) ||
                EF.Functions.Like(x.LastName, term)
            );
        }

        // Sorting
        dep = dep.ApplySorting(sort);

        // Count
        var totalCount = await dep.CountAsync(ct);

        // Paging + Projection
        var items = await dep
            .ApplyPaging(page)
            .ToListAsync(ct);

        return new PageResponse<Employee>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page.PageNumber,
            PageSize = page.PageSize
        };
    }
}
