using System.Globalization;
using System.Text.RegularExpressions;

namespace FileNet.WebFramework.ScanIngest;

public class ScanFileNameParser
{
    private static readonly Regex Rx = new(
        @"^(?<nc>\d{5,20})_(?<cat>\d{2})_(?<d>\d{8})_(?<seq>\d{3})\.(?<ext>[A-Za-z0-9]{1,10})$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public bool TryParse(string fileName, out ScanFileNameInfo? info, out string? error)
    {
        info = null; error = null;
        var m = Rx.Match(fileName);
        if (!m.Success) { error = "Filename does not match pattern."; return false; }

        var nc = m.Groups["nc"].Value;
        var catCode = m.Groups["cat"].Value;
        var d = m.Groups["d"].Value;
        var seqStr = m.Groups["seq"].Value;
        var ext = "." + m.Groups["ext"].Value.ToLowerInvariant();

        if (!DocumentCategoryCodes.TryFromCode(catCode, out var cat))
        { error = "Invalid category code."; return false; }

        if (!DateTime.TryParseExact(d, "yyyyMMdd", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var date))
        { error = "Invalid date segment."; return false; }

        if (!int.TryParse(seqStr, out var seq) || seq < 1 || seq > 999)
        { error = "Invalid sequence segment."; return false; }

        info = new ScanFileNameInfo
        {
            OriginalFileName = fileName,
            NationalCode = nc,
            Category = cat,
            DateUtc = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc),
            Sequence = seq,
            Extension = ext
        };
        return true;
    }
}
