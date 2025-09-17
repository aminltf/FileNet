using FileNet.Domain.Base;
using FileNet.Domain.Enums;

namespace FileNet.Domain.Entities;

public class Document : EntityBase
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;

    public string? Title { get; set; }
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = "application/octet-stream";
    public long Size { get; set; }

    public DocumentCategory Category { get; set; }

    public byte[] Data { get; set; } = default!;

    public DateTimeOffset UploadedOn { get; set; } = DateTimeOffset.UtcNow;

    public Document() { }
}
