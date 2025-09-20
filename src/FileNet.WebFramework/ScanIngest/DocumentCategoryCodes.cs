using FileNet.WebFramework.Enums;

namespace FileNet.WebFramework.ScanIngest;

public static class DocumentCategoryCodes
{
    public static string ToCode(DocumentCategory cat)
    {
        if (cat == DocumentCategory.Other) return "99";

        var v = (int)cat + 1;
        if (v < 0) v = 0;
        if (v > 99) v = 99;
        return v.ToString("00");
    }

    public static bool TryFromCode(string code, out DocumentCategory cat)
    {
        cat = DocumentCategory.Other;
        if (string.Equals(code, "99", StringComparison.Ordinal))
            return true; // Other

        if (!int.TryParse(code, out var v)) return false;
        if (v <= 0 || v > 99) return false;

        var val = v - 1;
        if (Enum.IsDefined(typeof(DocumentCategory), (byte)val))
        {
            cat = (DocumentCategory)(byte)val;
            return true;
        }

        cat = DocumentCategory.Other;
        return true;
    }
}
