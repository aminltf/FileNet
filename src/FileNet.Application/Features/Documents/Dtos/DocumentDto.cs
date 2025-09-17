using FileNet.Domain.Enums;

namespace FileNet.Application.Features.Documents.Dtos;

public class DocumentDto
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public string? Title { get; init; }
    public string FileName { get; init; } = default!;
    public string ContentType { get; init; } = default!;
    public long Size { get; init; }
    public DocumentCategory Category { get; init; }
    public DateTimeOffset UploadedOn { get; init; }
}
