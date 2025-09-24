using FileNet.Application.Abstractions.Persistence.Repositories;
using FileNet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileNet.Infrastructure.Persistence.Repositories;

public sealed class DocumentRepository(AppDbContext db) : IDocumentRepository
{
    public async Task<Document?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default)
    {
        var query = includeDeleted ? db.Documents.IgnoreQueryFilters() : db.Documents;
        return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task AddAsync(Document entity, CancellationToken ct = default) =>
        db.Documents.AddAsync(entity, ct).AsTask();

    public async Task<bool> ExistsActiveFileNameForEmployeeAsync(Guid employeeId, string fileName, Guid? excludingId = null, CancellationToken ct = default)
    {
        var q = db.Documents.Where(x => !x.IsDeleted && x.EmployeeId == employeeId && x.FileName == fileName);
        if (excludingId is Guid ex) q = q.Where(x => x.Id != ex);
        return await q.AnyAsync(ct);
    }
}
