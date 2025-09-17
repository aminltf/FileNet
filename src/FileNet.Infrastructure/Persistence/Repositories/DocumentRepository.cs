using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Domain.Entities;
using FileNet.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FileNet.Infrastructure.Persistence.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<DocumentRepository> _logger;

    public DocumentRepository(AppDbContext db, ILogger<DocumentRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Guid> UploadAsync(Document entity, CancellationToken ct)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        if (entity.EmployeeId == Guid.Empty)
            throw new ArgumentException("EmployeeId is required.", nameof(entity));

        if (entity.Data is null || entity.Data.Length == 0)
            throw new InvalidOperationException("Empty file.");

        const long maxAllowed = 50L * 1024 * 1024; // 50MB
        if (entity.Data.LongLength > maxAllowed)
            throw new InvalidOperationException("File size exceeds the allowed limit.");

        var employeeExists = await _db.Employees
            .AsNoTracking()
            .AnyAsync(e => e.Id == entity.EmployeeId, ct);

        if (!employeeExists)
            throw new KeyNotFoundException("Employee not found.");

        if (entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();

        entity.ContentType = string.IsNullOrWhiteSpace(entity.ContentType)
            ? "application/octet-stream"
            : entity.ContentType;

        await _db.Documents.AddAsync(entity, ct);

        _logger.LogInformation("Document {DocumentId} staged for upload for Employee {EmployeeId}",
            entity.Id, entity.EmployeeId);

        return entity.Id;
    }

    public async Task<IReadOnlyList<Document>> GetByEmployeeAsync(Guid employeeId, CancellationToken ct)
    {
        return await _db.Documents
            .Where(d => d.EmployeeId == employeeId)
            .OrderByDescending(d => d.UploadedOn)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public Task<Document?> DownloadAsync(Guid documentId, CancellationToken ct)
    {
        return _db.Documents
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == documentId, ct);
    }

    public async Task DeleteAsync(Guid documentId, CancellationToken ct)
    {
        var entity = await _db.Documents.FindAsync([documentId], ct);
        if (entity is null) return;

        _db.Documents.Remove(entity);
    }
}
