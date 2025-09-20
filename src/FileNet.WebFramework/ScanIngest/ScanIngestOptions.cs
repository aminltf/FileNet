namespace FileNet.WebFramework.ScanIngest;

public class ScanIngestOptions
{
    public bool Enabled { get; set; } = true;
    public string IncomingPath { get; set; } = default!;
    public string ProcessedPath { get; set; } = default!;
    public string UnmatchedPath { get; set; } = default!;
    public string ErrorPath { get; set; } = default!;
    public string[] AllowedExtensions { get; set; } = [".pdf", ".tif", ".tiff", ".jpg", ".jpeg", ".png"];
    public int PollIntervalSeconds { get; set; } = 5;
    public int FileReadyDelaySeconds { get; set; } = 3;
    public int MaxFileSizeMB { get; set; } = 100;
    public string NamePattern { get; set; } = "{NationalCode}_{CategoryNN}_{yyyyMMdd}_{NNN}{ext}";
}
