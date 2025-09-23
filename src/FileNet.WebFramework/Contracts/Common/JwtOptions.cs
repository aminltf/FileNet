namespace FileNet.WebFramework.Contracts.Common;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string Key { get; set; } = default!; // 32+ chars
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 7;

    public CookieNames Cookies { get; set; } = new();
    public class CookieNames
    {
        public string Access { get; set; } = "fn_access";
        public string Refresh { get; set; } = "fn_refresh";
    }
}
