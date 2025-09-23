using FileNet.WebFramework.Contracts.Common;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace FileNet.WebApplication.Pages.Account;

public class RefreshModel(ITokenService tokens, IOptions<JwtOptions> jwtOpt) : PageModel
{
    private readonly JwtOptions _jwt = jwtOpt.Value;

    public IActionResult OnGet() => Unauthorized();
    public async Task<IActionResult> OnPostAsync()
    {
        if (!Request.Cookies.TryGetValue(_jwt.Cookies.Refresh, out var refreshToken))
            return Unauthorized();

        var ip = GetClientIp(HttpContext);
        var (access, exp, newRefresh) = await tokens.RotateAsync(refreshToken, ip);

        Response.Cookies.Append(_jwt.Cookies.Access, access, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = exp
        });
        Response.Cookies.Append(_jwt.Cookies.Refresh, newRefresh.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = newRefresh.Expires
        });

        return new JsonResult(new { ok = true, accessExpires = exp });
    }

    private static string GetClientIp(HttpContext ctx)
    {
        if (ctx.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded.ToString().Split(',').FirstOrDefault()?.Trim() ?? ctx.Connection.RemoteIpAddress?.ToString() ?? "-";
        return ctx.Connection.RemoteIpAddress?.ToString() ?? "-";
    }
}
