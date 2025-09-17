using FileNet.Domain.Entities;

namespace FileNet.Application.Common.Abstractions.Repositories;

public interface IDocumentRepository
{
    Task<Guid> UploadAsync(Document entity, CancellationToken ct);
    Task<IReadOnlyList<Document>> GetByEmployeeAsync(Guid employeeId, CancellationToken ct);
    Task<Document?> DownloadAsync(Guid documentId, CancellationToken ct);
    Task DeleteAsync(Guid documentId, CancellationToken ct);
}
