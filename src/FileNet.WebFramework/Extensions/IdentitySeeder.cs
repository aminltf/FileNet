using FileNet.WebFramework.Contexts;
using FileNet.WebFramework.Contracts.Common;
using FileNet.WebFramework.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FileNet.WebFramework.Extensions;

public static class IdentitySeeder
{
    /// <summary>
    /// Applies pending migrations for IdentityContext and seeds roles/users.
    /// </summary>
    public static async Task SeedIdentityAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var ctx = sp.GetRequiredService<IdentityContext>();
        await ctx.Database.MigrateAsync(); // Just IdentityContext

        var userMgr = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        // Roles
        foreach (var role in new[] { AppRoles.Admin, AppRoles.Manager, AppRoles.User })
            if (!await roleMgr.RoleExistsAsync(role))
                await roleMgr.CreateAsync(new IdentityRole<Guid>(role));

        // Admin
        var admin = await userMgr.FindByNameAsync("admin");
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@local",
                DisplayName = "System Admin",
                EmailConfirmed = true
            };
            await userMgr.CreateAsync(admin, "P@ssw0rd!");
            await userMgr.AddToRoleAsync(admin, AppRoles.Admin);
        }

        // Manager
        var manager = await userMgr.FindByNameAsync("manager");
        if (manager is null)
        {
            manager = new ApplicationUser
            {
                UserName = "manager",
                Email = "manager@local",
                DisplayName = "HR Manager",
                EmailConfirmed = true
            };
            await userMgr.CreateAsync(manager, "P@ssw0rd!");
            await userMgr.AddToRoleAsync(manager, AppRoles.Manager);
        }

        // User
        var user = await userMgr.FindByNameAsync("user");
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = "user",
                Email = "user@local",
                DisplayName = "HR user",
                EmailConfirmed = true
            };
            await userMgr.CreateAsync(user, "P@ssw0rd!");
            await userMgr.AddToRoleAsync(user, AppRoles.Manager);
        }
    }
}
