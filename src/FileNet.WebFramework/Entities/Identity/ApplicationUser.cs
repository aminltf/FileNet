using Microsoft.AspNetCore.Identity;

namespace FileNet.WebFramework.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? DisplayName { get; set; }    
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
