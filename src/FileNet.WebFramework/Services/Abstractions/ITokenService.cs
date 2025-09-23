using FileNet.WebFramework.Entities.Identity;

namespace FileNet.WebFramework.Services.Abstractions;

public interface ITokenService
{
    Task<(string Token, DateTime Expires)> CreateAccessTokenAsync(ApplicationUser user, CancellationToken ct = default);
    Task<RefreshToken> CreateAndSaveRefreshTokenAsync(ApplicationUser user, string ip, CancellationToken ct = default);
    Task<(string Token, DateTime Expires, RefreshToken NewRefresh)> RotateAsync(string oldRefreshToken, string ip, CancellationToken ct = default);
    Task RevokeAsync(string refreshToken, string ip, CancellationToken ct = default);
}
