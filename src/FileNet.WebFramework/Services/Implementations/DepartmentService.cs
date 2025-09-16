using FileNet.WebFramework.Contexts;
using FileNet.WebFramework.Contracts.Departments;
using FileNet.WebFramework.Contracts.Employees;
using FileNet.WebFramework.Entities;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FileNet.WebFramework.Services.Implementations;

public class DepartmentService(AppDbContext db) : IDepartmentService
{
    public async Task<Guid> CreateAsync(DepartmentCreateDto dto, CancellationToken ct)
    {
        var exists = await db.Departments
            .AnyAsync(e => e.Name == dto.Name, ct);
        if (exists)
            throw new InvalidOperationException("Name already exists.");

        var entity = new Department
        {
            Code = dto.Code.Trim(),
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim()
        };

        db.Departments.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken ct)
    {
        return await db.Departments
            .OrderBy(e => e.Name).ThenBy(e => e.Name)
            .Select(e => new DepartmentDto
            {
                Id = e.Id,
                Code = e.Code,
                Name = e.Name,
                Description = e.Description,
                EmployeeCount = db.Employees.Count(d => d.DepartmentId == e.Id)
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<DepartmentDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await db.Departments
            .Where(e => e.Id == id)
            .Select(e => new DepartmentDto
            {
                Id = e.Id,
                Code = e.Code,
                Name = e.Name,
                Description = e.Description,
                EmployeeCount = db.Employees.Count(d => d.DepartmentId == e.Id)
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);
    }

    public async Task UpdateAsync(DepartmentUpdateDto dto, CancellationToken ct)
    {
        var entity = await db.Departments.FirstOrDefaultAsync(e => e.Id == dto.Id, ct)
                     ?? throw new KeyNotFoundException("Department not found.");

        if (!string.Equals(entity.Name, dto.Name, StringComparison.Ordinal))
        {
            var exists = await db.Departments.AnyAsync(e => e.Name == dto.Name && e.Id != dto.Id, ct);
            if (exists) throw new InvalidOperationException("Department name already exists.");
        }

        entity.Code = dto.Code.Trim();
        entity.Name = dto.Name.Trim();
        entity.Description = dto.Description?.Trim();

        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await db.Departments.FindAsync([id], ct);
        if (entity is null) return;

        db.Departments.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<DepartmentLookupDto>> GetLookupAsync(CancellationToken ct)
    {
        return await db.Departments
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .Select(e => new DepartmentLookupDto
            {
                Id = e.Id,
                Name = e.Name
            })
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<EmployeeDto>> GetEmployeesAsync(Guid departmentId, CancellationToken ct)
    {
        return await db.Employees
            .Where(e => e.DepartmentId == departmentId)
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
}
