using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Queryable;
using FileNet.Domain.Entities;
using FileNet.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FileNet.Infrastructure.Persistence.Repositories;

public class DepartmentRepository : EfRepository<Department>, IDepartmentRepository
{
    private readonly AppDbContext _context;

    public DepartmentRepository(AppDbContext db) : base(db)
    {
        _context = db;
    }

    public async Task<IReadOnlyList<Department>> GetLookupAsync(CancellationToken ct)
    {
        return await _context.Departments
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .Select(e => new Department
            {
                Id = e.Id,
                Name = e.Name
            })
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Employee>> GetEmployeesAsync(Guid departmentId, CancellationToken ct)
    {
        return await _context.Employees
            .Where(x => x.DepartmentId == departmentId)
            .OrderBy(x => x.LastName).ThenBy(e => e.FirstName)
            .Select(x => new Employee
            {
                Id = x.Id,
                NationalCode = x.NationalCode,
                FirstName = x.FirstName,
                LastName = x.LastName
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<PageResponse<Department>> GetPagedAsync(
        PageRequest page,
        SearchRequest search,
        SortOptions sort,
        CancellationToken ct)
    {
        var dep = _context.Departments
            .AsQueryable()
            .AsNoTracking();

        // Search
        if (!string.IsNullOrWhiteSpace(search?.SearchTerm))
        {
            var term = $"%{search.SearchTerm.Trim()}%";
            dep = dep.Where(x =>
                EF.Functions.Like(x.Code, term) ||
                EF.Functions.Like(x.Name, term) ||
                EF.Functions.Like(x.Description, term)
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

        return new PageResponse<Department>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page.PageNumber,
            PageSize = page.PageSize
        };
    }
}
