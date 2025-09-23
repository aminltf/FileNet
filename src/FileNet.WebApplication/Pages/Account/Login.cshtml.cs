using System.ComponentModel.DataAnnotations;
using FileNet.WebFramework.Contracts.Common;
using FileNet.WebFramework.Entities.Identity;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace FileNet.WebApplication.Pages.Account;

public class LoginModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    ITokenService tokens,
    IHttpContextAccessor accessor,
    IOptions<JwtOptions> jwtOptions) : PageModel
{
    public string? Error { get; set; }
    [BindProperty] public InputModel Input { get; set; } = new();
    [BindProperty(SupportsGet = true)] public string? ReturnUrl { get; set; }

    private readonly JwtOptions _jwt = jwtOptions.Value;

    public class InputModel
    {
        [Display(Name = "نام‌کاربری")]
        public string UserName { get; set; } = default!;
        [Display(Name = "رمز عبور")]
        public string Password { get; set; } = default!;
        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var user = await userManager.FindByNameAsync(Input.UserName)
                   ?? await userManager.FindByEmailAsync(Input.UserName);

        if (user is null)
        {
            Error = "نام کاربری/ایمیل یا کلمه عبور نادرست است.";
            return Page();
        }

        var result = await signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            Error = "نام کاربری/ایمیل یا کلمه عبور نادرست است.";
            return Page();
        }

        var (access, exp) = await tokens.CreateAccessTokenAsync(user);
        var ip = GetClientIp(accessor.HttpContext!);
        var refresh = await tokens.CreateAndSaveRefreshTokenAsync(user, ip);

        Response.Cookies.Append(_jwt.Cookies.Access, access, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = exp
        });
        Response.Cookies.Append(_jwt.Cookies.Refresh, refresh.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = refresh.Expires
        });

        return LocalRedirect(ReturnUrl ?? "/");
    }

    private static string GetClientIp(HttpContext ctx)
    {
        if (ctx.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded.ToString().Split(',').FirstOrDefault()?.Trim() ?? ctx.Connection.RemoteIpAddress?.ToString() ?? "-";
        return ctx.Connection.RemoteIpAddress?.ToString() ?? "-";
    }
}
