using FileNet.WebFramework.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FileNet.WebFramework.Contexts;

public class IdentityContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<LoginLog> LoginLogs => Set<LoginLog>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<ApplicationUser>(cfg =>
        {
            cfg.ToTable("Users");
            cfg.Property(x => x.DisplayName).HasMaxLength(100);
        });

        b.Entity<IdentityRole<Guid>>().ToTable("Roles");
        b.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        b.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        b.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        b.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        b.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        b.Entity<RefreshToken>(cfg =>
        {
            cfg.ToTable("RefreshTokens");
            cfg.HasKey(x => x.Id);
            cfg.Property(x => x.Token).IsRequired().HasMaxLength(400);
            cfg.Property(x => x.CreatedByIp).HasMaxLength(64);
            cfg.HasIndex(x => x.Token).IsUnique();
        });

        b.Entity<LoginLog>(cfg =>
        {
            cfg.ToTable("LoginLogs");
            cfg.HasKey(x => x.Id);
            cfg.Property(x => x.Ip).HasMaxLength(64);
            cfg.Property(x => x.UserAgent).HasMaxLength(512);
            cfg.HasIndex(x => x.UserId);
            cfg.HasIndex(x => x.AtUtc);
        });
    }
}
