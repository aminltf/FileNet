using FileNet.Application.Abstractions.Persistence.Repositories;
using FileNet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileNet.Infrastructure.Persistence.Repositories;

public sealed class DepartmentRepository(AppDbContext db) : IDepartmentRepository
{
    public async Task<Department?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default)
    {
        var query = includeDeleted ? db.Departments.IgnoreQueryFilters() : db.Departments;
        return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task AddAsync(Department entity, CancellationToken ct = default) =>
        db.Departments.AddAsync(entity, ct).AsTask();

    public async Task<bool> ExistsActiveByCodeAsync(string code, Guid? excludingId = null, CancellationToken ct = default)
    {
        var q = db.Departments.Where(x => !x.IsDeleted && x.Code == code);
        if (excludingId is Guid ex) q = q.Where(x => x.Id != ex);
        return await q.AnyAsync(ct);
    }
}
