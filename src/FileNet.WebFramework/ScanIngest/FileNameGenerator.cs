using System.Text.RegularExpressions;
using FileNet.WebFramework.Enums;

namespace FileNet.WebFramework.ScanIngest;

public static class FileNameGenerator
{
    public static string Generate(string nationalCode, DocumentCategory category, DateTime dateUtc, int sequence, string extensionWithDot)
    {
        var catCode = DocumentCategoryCodes.ToCode(category);
        var d = dateUtc.ToString("yyyyMMdd");
        var seq = Math.Clamp(sequence, 1, 999).ToString("000");
        var ext = extensionWithDot.StartsWith('.') ? extensionWithDot.ToLowerInvariant() : "." + extensionWithDot.ToLowerInvariant();
        return $"{nationalCode}_{catCode}_{d}_{seq}{ext}";
    }

    public static int GetNextSequence(string folder, string nationalCode, DocumentCategory category, DateTime dateUtc, string extensionWithDot)
    {
        var catCode = DocumentCategoryCodes.ToCode(category);
        var datePart = dateUtc.ToString("yyyyMMdd");
        var ext = extensionWithDot.StartsWith('.') ? extensionWithDot.ToLowerInvariant() : "." + extensionWithDot.ToLowerInvariant();

        var prefix = $"{nationalCode}_{catCode}_{datePart}_";
        var files = Directory.Exists(folder)
            ? Directory.EnumerateFiles(folder, $"{prefix}*{ext}", SearchOption.TopDirectoryOnly)
            : Enumerable.Empty<string>();

        int maxSeq = 0;
        var rx = new Regex($@"^{Regex.Escape(prefix)}(?<seq>\d{{3}}){Regex.Escape(ext)}$", RegexOptions.Compiled);
        foreach (var f in files)
        {
            var name = Path.GetFileName(f);
            var m = rx.Match(name);
            if (m.Success && int.TryParse(m.Groups["seq"].Value, out var seq))
                if (seq > maxSeq) maxSeq = seq;
        }
        return Math.Clamp(maxSeq + 1, 1, 999);
    }
}
