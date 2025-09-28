using FileNet.WebFramework.Contexts;
using FileNet.WebFramework.Contracts.Common;
using FileNet.WebFramework.Contracts.Dependents;
using FileNet.WebFramework.Contracts.Employees;
using FileNet.WebFramework.Entities;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FileNet.WebFramework.Services.Implementations;

public class DependentService(AppDbContext db) : IDependentService
{
    public async Task<Guid> CreateAsync(DependentCreateDto dto, CancellationToken ct)
    {
        var exists = await db.Dependents
            .AnyAsync(x => x.NationalCode == dto.NationalCode, ct);
        if (exists)
            throw new InvalidOperationException("National code already exists.");

        var entity = new Dependent
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            NationalCode = dto.NationalCode.Trim(),
            Gender = dto.Gender,
            Relation = dto.Relation,
            EmployeeId = dto.EmployeeId
        };

        db.Dependents.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<IReadOnlyList<DependentDto>> GetAllAsync(CancellationToken ct)
    {
        return await db.Dependents
            .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
            .Select(x => new DependentDto
            {
                Id = x.Id,
                NationalCode = x.NationalCode,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Relation = x.Relation,
                EmployeeFullName = x.Employee.FirstName + " " + x.Employee.LastName,
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<DependentDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await db.Dependents
            .Where(x => x.Id == id)
            .Select(x => new DependentDto
            {
                Id = x.Id,
                NationalCode = x.NationalCode,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Gender = x.Gender,
                Relation = x.Relation,
                EmployeeId = x.EmployeeId,
                EmployeeFullName = x.Employee.FirstName + " " + x.Employee.LastName
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);
    }

    public async Task UpdateAsync(DependentUpdateDto dto, CancellationToken ct)
    {
        var entity = await db.Dependents.FirstOrDefaultAsync(x => x.Id == dto.Id, ct)
                     ?? throw new KeyNotFoundException("Dependent not found.");

        if (!string.Equals(entity.NationalCode, dto.NationalCode, StringComparison.Ordinal))
        {
            var exists = await db.Dependents.AnyAsync(x => x.NationalCode == dto.NationalCode && x.Id != dto.Id, ct);
            if (exists) throw new InvalidOperationException("National code already exists.");
        }

        entity.NationalCode = dto.NationalCode.Trim();
        entity.FirstName = dto.FirstName.Trim();
        entity.LastName = dto.LastName.Trim();
        entity.Gender = dto.Gender;
        entity.Relation = dto.Relation;
        entity.EmployeeId = dto.EmployeeId;

        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await db.Dependents.FindAsync([id], ct);
        if (entity is null) return;

        db.Dependents.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<DependentDto>> GetByEmployeeAsync(Guid employeeId, CancellationToken ct)
    {
        return await db.Dependents
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(d => d.CreatedOn)
            .Select(x => new DependentDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                NationalCode = x.NationalCode,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Gender = x.Gender,
                Relation = x.Relation,
                EmployeeFullName = x.Employee.FirstName + " " + x.Employee.LastName,
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<PageResponse<DependentDto>> GetPagedAsync(
        PagedRequest request,
        CancellationToken ct)
    {
        IQueryable<DependentDto> query = db.Dependents
            .AsNoTracking()
            .Select(x => new DependentDto
            {
                Id = x.Id,
                NationalCode = x.NationalCode,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Gender = x.Gender,
                Relation = x.Relation,
                EmployeeId = x.EmployeeId,
                EmployeeFullName = x.Employee.FirstName + " " + x.Employee.LastName,
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

        return new PageResponse<DependentDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
