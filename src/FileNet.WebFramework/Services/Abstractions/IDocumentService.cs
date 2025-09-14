using FileNet.WebFramework.Contracts.Documents;

namespace FileNet.WebFramework.Services.Abstractions;

public interface IDocumentService
{
    Task<Guid> UploadAsync(DocumentUploadDto dto, CancellationToken ct);
    Task<IReadOnlyList<DocumentDto>> GetByEmployeeAsync(Guid employeeId, CancellationToken ct);
    Task<DocumentDownloadDto?> DownloadAsync(Guid documentId, CancellationToken ct);
    Task DeleteAsync(Guid documentId, CancellationToken ct);
}
