using FileNet.WebFramework.Contexts;
using FileNet.WebFramework.Contracts.Common;
using FileNet.WebFramework.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace FileNet.WebApplication.Pages.Account;

public class ProfileModel(
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor accessor,
    IdentityContext idDb,
    IOptions<JwtOptions> jwtOpt) : PageModel
{
    public string? UserName { get; private set; }
    public string? DisplayName { get; private set; }
    public string? Email { get; private set; }
    public string Roles { get; private set; } = "-";
    public string ClientIp { get; private set; } = "-";
    public DateTime? AccessTokenExpiresUtc { get; private set; }
    public List<LoginLogVm> LoginLogs { get; private set; } = new();

    private readonly JwtOptions _jwt = jwtOpt.Value;

    public async Task OnGetAsync()
    {
        var http = accessor.HttpContext!;
        ClientIp = GetClientIp(http);

        var user = await userManager.GetUserAsync(User);
        if (user is null) return;

        UserName = user.UserName;
        DisplayName = user.DisplayName ?? user.UserName;
        Email = user.Email;

        var roles = await userManager.GetRolesAsync(user);
        Roles = roles.Count == 0 ? "-" : string.Join(", ", roles);

        if (Request.Cookies.TryGetValue(_jwt.Cookies.Access, out var at))
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(at);
                var expClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value
                               ?? token.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
                if (long.TryParse(expClaim, out var expUnix))
                    AccessTokenExpiresUtc = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            }
            catch { /* ignore parse errors */ }
        }

        LoginLogs = await idDb.LoginLogs
            .Where(x => x.UserId == user.Id)
            .OrderByDescending(x => x.AtUtc)
            .Take(20)
            .Select(x => new LoginLogVm { AtUtc = x.AtUtc, Ip = x.Ip, UserAgent = x.UserAgent ?? "-" })
            .ToListAsync();
    }

    private static string GetClientIp(HttpContext ctx)
    {
        if (ctx.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded.ToString().Split(',').FirstOrDefault()?.Trim() ?? ctx.Connection.RemoteIpAddress?.ToString() ?? "-";
        return ctx.Connection.RemoteIpAddress?.ToString() ?? "-";
    }

    public record LoginLogVm
    {
        public DateTime AtUtc { get; set; }
        public string Ip { get; set; } = "-";
        public string UserAgent { get; set; } = "-";
    }
}
