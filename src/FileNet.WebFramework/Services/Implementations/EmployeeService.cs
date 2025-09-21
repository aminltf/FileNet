using FileNet.WebFramework.Contexts;
using FileNet.WebFramework.Contracts.Common;
using FileNet.WebFramework.Contracts.Employees;
using FileNet.WebFramework.Entities;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FileNet.WebFramework.Services.Implementations;

public class EmployeeService(AppDbContext db) : IEmployeeService
{
    public async Task<Guid> CreateAsync(EmployeeCreateDto dto, CancellationToken ct)
    {
        var exists = await db.Employees
            .AnyAsync(e => e.NationalCode == dto.NationalCode, ct);
        if (exists)
            throw new InvalidOperationException("National code already exists.");

        var entity = new Employee
        {
            NationalCode = dto.NationalCode.Trim(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Gender = dto.Gender,
            DepartmentId = dto.DepartmentId
        };

        db.Employees.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken ct)
    {
        return await db.Employees
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                NationalCode = e.NationalCode,
                FirstName = e.FirstName,
                LastName = e.LastName,
                DocumentCount = db.Documents.Count(d => d.EmployeeId == e.Id)
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await db.Employees
            .Where(e => e.Id == id)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                NationalCode = e.NationalCode,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Gender = e.Gender,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name,
                DocumentCount = db.Documents.Count(d => d.EmployeeId == e.Id)
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);
    }

    public async Task UpdateAsync(EmployeeUpdateDto dto, CancellationToken ct)
    {
        var entity = await db.Employees.FirstOrDefaultAsync(e => e.Id == dto.Id, ct)
                     ?? throw new KeyNotFoundException("Employee not found.");

        if (!string.Equals(entity.NationalCode, dto.NationalCode, StringComparison.Ordinal))
        {
            var exists = await db.Employees.AnyAsync(e => e.NationalCode == dto.NationalCode && e.Id != dto.Id, ct);
            if (exists) throw new InvalidOperationException("National code already exists.");
        }

        entity.NationalCode = dto.NationalCode.Trim();
        entity.FirstName = dto.FirstName.Trim();
        entity.LastName = dto.LastName.Trim();
        entity.Gender = dto.Gender;
        entity.DepartmentId = dto.DepartmentId;

        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await db.Employees.FindAsync([id], ct);
        if (entity is null) return;

        db.Employees.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task<PageResponse<EmployeeDto>> GetPagedAsync(
        PagedRequest request,
        CancellationToken ct)
    {
        IQueryable<EmployeeDto> query = db.Employees
            .AsNoTracking()
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                NationalCode = e.NationalCode,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Gender = e.Gender,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department!.Name,
                DocumentCount = e.Documents.Count
            });

        // Filter/Search
        query = query.ApplySearching(request);

        // Count
        var totalCount = await query.CountAsync(ct);

        // Sorting
        if (string.IsNullOrWhiteSpace(request.SortColumn))
            query = query.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
        else
            query = query.ApplySorting(request);

        // Paging
        var items = await query.ApplyPaging(request).ToListAsync(ct);

        int pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
        int pageSize = request.PageSize > 0 ? Math.Min(request.PageSize, 10000) : 10;

        return new PageResponse<EmployeeDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
