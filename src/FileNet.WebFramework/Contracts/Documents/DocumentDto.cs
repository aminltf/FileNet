using FileNet.WebFramework.Enums;

namespace FileNet.WebFramework.Contracts.Documents;

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
