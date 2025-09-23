using FileNet.WebFramework.Contracts.Common;
using FileNet.WebFramework.Entities.Identity;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace FileNet.WebApplication.Pages.Account;

public class LogoutModel(SignInManager<ApplicationUser> signInManager, ITokenService tokens, IOptions<JwtOptions> jwtOpt) : PageModel
{
    private readonly JwtOptions _jwt = jwtOpt.Value;

    public async Task<IActionResult> OnPostAsync()
    {
        if (Request.Cookies.TryGetValue(_jwt.Cookies.Refresh, out var refresh))
            await tokens.RevokeAsync(refresh, GetIp(HttpContext));

        await signInManager.SignOutAsync();

        Response.Cookies.Delete(_jwt.Cookies.Access);
        Response.Cookies.Delete(_jwt.Cookies.Refresh);

        return LocalRedirect("/");
    }

    private static string GetIp(HttpContext ctx) =>
        ctx.Connection.RemoteIpAddress?.ToString() ?? "-";
}
