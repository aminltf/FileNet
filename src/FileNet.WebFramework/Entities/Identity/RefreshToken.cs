namespace FileNet.WebFramework.Entities.Identity;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Token { get; set; } = default!;
    public DateTime Created { get; set; }
    public string CreatedByIp { get; set; } = default!;
    public DateTime Expires { get; set; }
    public DateTime? Revoked { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }

    public bool IsActive => Revoked == null && DateTime.UtcNow <= Expires;
}
