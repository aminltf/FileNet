using FileNet.WebFramework.Enums;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FileNet.WebFramework.ScanIngest;

public class ScanFileNameParser
{
    private static readonly Regex Rx = new(
        @"^(?<nc>\d{5,20})__(?<cat>[A-Za-z]+)__(?<ts>\d{14})(?:__(?<title>.+))?\.(?<ext>[A-Za-z0-9]{1,10})$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Dictionary<string, DocumentCategory> CatMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["ID"] = DocumentCategory.Identity,
            ["IDENTITY"] = DocumentCategory.Identity,

            ["CONTRACT"] = DocumentCategory.Contract,
            ["CTR"] = DocumentCategory.Contract,

            ["EDU"] = DocumentCategory.Education,
            ["EDUCATION"] = DocumentCategory.Education,
            ["CERT"] = DocumentCategory.Education,

            ["OTHER"] = DocumentCategory.Other,
            ["MISC"] = DocumentCategory.Other
        };

    public bool TryParse(string fileName, out ScanFileNameInfo? info, out string? error)
    {
        info = null;
        error = null;

        var m = Rx.Match(fileName);
        if (!m.Success)
        {
            error = "Filename does not match expected pattern.";
            return false;
        }

        var nc = m.Groups["nc"].Value;
        var catToken = m.Groups["cat"].Value;
        var ts = m.Groups["ts"].Value;
        var titleRaw = m.Groups["title"].Success ? m.Groups["title"].Value : null;
        var ext = "." + m.Groups["ext"].Value.ToLowerInvariant();

        if (!CatMap.TryGetValue(catToken, out var cat))
            cat = DocumentCategory.Other;

        if (!DateTime.TryParseExact(ts, "yyyyMMddHHmmss", CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var utc))
        {
            error = "Invalid timestamp segment.";
            return false;
        }

        string? title = titleRaw is null ? null :
            titleRaw.Replace('_', ' ').Trim();

        info = new ScanFileNameInfo
        {
            OriginalFileName = fileName,
            NationalCode = nc,
            Category = cat,
            TimestampUtc = DateTime.SpecifyKind(utc, DateTimeKind.Utc),
            Title = string.IsNullOrWhiteSpace(title) ? null : title,
            Extension = ext
        };
        return true;
    }
}
