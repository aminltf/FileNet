using FileNet.Application.Abstractions.Persistence.Repositories;
using FileNet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileNet.Infrastructure.Persistence.Repositories;

public sealed class EmployeeRepository(AppDbContext db) : IEmployeeRepository
{
    public async Task<Employee?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default)
    {
        var query = includeDeleted ? db.Employees.IgnoreQueryFilters() : db.Employees;
        return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task AddAsync(Employee entity, CancellationToken ct = default) =>
        db.Employees.AddAsync(entity, ct).AsTask();

    public async Task<bool> ExistsActiveByNationalCodeAsync(string nationalCode, Guid? excludingId = null, CancellationToken ct = default)
    {
        var q = db.Employees.Where(x => !x.IsDeleted && x.NationalCode == nationalCode);
        if (excludingId is Guid ex) q = q.Where(x => x.Id != ex);
        return await q.AnyAsync(ct);
    }

    public async Task<bool> ExistsActiveByEmployeeCodeAsync(string employeeCode, Guid? excludingId = null, CancellationToken ct = default)
    {
        var q = db.Employees.Where(x => !x.IsDeleted && x.EmployeeCode == employeeCode);
        if (excludingId is Guid ex) q = q.Where(x => x.Id != ex);
        return await q.AnyAsync(ct);
    }
}
