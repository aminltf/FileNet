using FileNet.WebFramework.Enums;

namespace FileNet.WebFramework.ScanIngest;

public class ScanFileNameInfo
{
    public string OriginalFileName { get; init; } = default!;
    public string NationalCode { get; init; } = default!;
    public DocumentCategory Category { get; init; } = DocumentCategory.Other;
    public DateTime DateUtc { get; init; }
    public int Sequence { get; init; }
    public string Extension { get; init; } = default!;
}
