namespace FileNet.WebFramework.ScanIngest;

public static class MimeHelper
{
    private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        [".pdf"] = "application/pdf",
        [".tif"] = "image/tiff",
        [".tiff"] = "image/tiff",
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".txt"] = "text/plain"
    };

    public static string FromExtension(string ext)
        => Map.TryGetValue(ext, out var ct) ? ct : "application/octet-stream";
}
