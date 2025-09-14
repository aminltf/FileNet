using FileNet.WebFramework.Enums;

namespace FileNet.WebFramework.Entities;

public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;

    public string? Title { get; set; }
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = "application/octet-stream";
    public long Size { get; set; }

    public DocumentCategory Category { get; set; }

    public byte[] Data { get; set; } = default!;

    public DateTimeOffset UploadedOn { get; set; } = DateTimeOffset.UtcNow;

    public Document()
    {

    }
}
