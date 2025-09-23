using FileNet.WebFramework.Contexts;
using FileNet.WebFramework.Contracts.Common;
using FileNet.WebFramework.Entities.Identity;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FileNet.WebFramework.Services.Implementations;

public class TokenService(
    UserManager<ApplicationUser> userManager,
    IdentityContext db,
    IOptions<JwtOptions> jwtOpt)
    : ITokenService
{
    private readonly JwtOptions _opt = jwtOpt.Value;

    public async Task<(string Token, DateTime Expires)> CreateAccessTokenAsync(ApplicationUser user, CancellationToken ct = default)
    {
        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? ""),
            new("display_name", user.DisplayName ?? user.UserName ?? "")
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_opt.AccessTokenMinutes);

        var token = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    public async Task<RefreshToken> CreateAndSaveRefreshTokenAsync(ApplicationUser user, string ip, CancellationToken ct = default)
    {
        var rt = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Created = DateTime.UtcNow,
            CreatedByIp = ip,
            Expires = DateTime.UtcNow.AddDays(_opt.RefreshTokenDays)
        };
        db.RefreshTokens.Add(rt);
        await db.SaveChangesAsync(ct);
        return rt;
    }

    public async Task<(string Token, DateTime Expires, RefreshToken NewRefresh)> RotateAsync(string oldRefreshToken, string ip, CancellationToken ct = default)
    {
        var existing = await db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == oldRefreshToken, ct);
        if (existing is null || !existing.IsActive) throw new SecurityTokenException("Invalid refresh token.");

        var user = await db.Users.FirstAsync(u => u.Id == existing.UserId, ct);

        existing.Revoked = DateTime.UtcNow;
        existing.RevokedByIp = ip;
        var newRt = await CreateAndSaveRefreshTokenAsync(user, ip, ct);
        existing.ReplacedByToken = newRt.Token;
        await db.SaveChangesAsync(ct);

        var (access, exp) = await CreateAccessTokenAsync(user, ct);
        return (access, exp, newRt);
    }

    public async Task RevokeAsync(string refreshToken, string ip, CancellationToken ct = default)
    {
        var rt = await db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken, ct);
        if (rt is null || !rt.IsActive) return;
        rt.Revoked = DateTime.UtcNow;
        rt.RevokedByIp = ip;
        await db.SaveChangesAsync(ct);
    }
}
