namespace FileNet.WebFramework.Contracts.Documents;

public class DocumentDownloadDto
{
    public string FileName { get; init; } = default!;
    public string ContentType { get; init; } = "application/octet-stream";
    public byte[] Data { get; init; } = default!;
}
