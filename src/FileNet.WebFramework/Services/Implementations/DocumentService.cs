using FileNet.WebFramework.Contexts;
using FileNet.WebFramework.Contracts.Documents;
using FileNet.WebFramework.Entities;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FileNet.WebFramework.Services.Implementations;

public class DocumentService(AppDbContext db, ILogger<DocumentService> logger) : IDocumentService
{
    public async Task<Guid> UploadAsync(DocumentUploadDto dto, CancellationToken ct)
    {
        var employeeExists = await db.Employees.AnyAsync(e => e.Id == dto.EmployeeId, ct);
        if (!employeeExists) throw new KeyNotFoundException("Employee not found.");

        if (dto.File.Length <= 0)
            throw new InvalidOperationException("Empty file.");

        const long maxAllowed = 50L * 1024 * 1024; // 50MB
        if (dto.File.Length > maxAllowed)
            throw new InvalidOperationException("File size exceeds the allowed limit.");

        byte[] data;
        using (var ms = new MemoryStream())
        {
            await dto.File.CopyToAsync(ms, ct);
            data = ms.ToArray();
        }

        var entity = new Document
        {
            EmployeeId = dto.EmployeeId,
            Title = dto.Title?.Trim(),
            Category = dto.Category,
            FileName = Path.GetFileName(dto.File.FileName),
            ContentType = string.IsNullOrWhiteSpace(dto.File.ContentType)
                          ? "application/octet-stream" : dto.File.ContentType,
            Size = dto.File.Length,
            Data = data
        };

        db.Documents.Add(entity);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Document {DocumentId} uploaded for Employee {EmployeeId}", entity.Id, dto.EmployeeId);
        return entity.Id;
    }

    public async Task<IReadOnlyList<DocumentDto>> GetByEmployeeAsync(Guid employeeId, CancellationToken ct)
    {
        return await db.Documents
            .Where(d => d.EmployeeId == employeeId)
            .OrderByDescending(d => d.UploadedOn)
            .Select(d => new DocumentDto
            {
                Id = d.Id,
                EmployeeId = d.EmployeeId,
                Title = d.Title,
                FileName = d.FileName,
                ContentType = d.ContentType,
                Size = d.Size,
                Category = d.Category,
                UploadedOn = d.UploadedOn,
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<DocumentDownloadDto?> DownloadAsync(Guid documentId, CancellationToken ct)
    {
        var doc = await db.Documents
            .Where(d => d.Id == documentId)
            .Select(d => new { d.FileName, d.ContentType, d.Data })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (doc is null) return null;

        return new DocumentDownloadDto
        {
            FileName = doc.FileName,
            ContentType = doc.ContentType,
            Data = doc.Data
        };
    }

    public async Task DeleteAsync(Guid documentId, CancellationToken ct)
    {
        var entity = await db.Documents.FindAsync([documentId], ct);
        if (entity is null) return;

        db.Documents.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}
