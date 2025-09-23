using System.Text;
using FileNet.WebFramework.Contexts;
using FileNet.WebFramework.Contracts.Common;
using FileNet.WebFramework.Entities.Identity;
using FileNet.WebFramework.Services.Abstractions;
using FileNet.WebFramework.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FileNet.WebFramework;

public static class DependencyInjection
{
    public static IServiceCollection AddWebFramework(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(cfg.GetConnectionString("Default")));

        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IDepartmentService, DepartmentService>();

        // Identity
        var cs = cfg.GetConnectionString("Default")!;
        services.AddDbContext<IdentityContext>(o => o.UseSqlServer(cs));

        services.Configure<JwtOptions>(cfg.GetSection(JwtOptions.SectionName));

        services
            .AddIdentityCore<ApplicationUser>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddSignInManager();

        // JWT Bearer
        var jwt = cfg.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

        // Authentication + Identity cookies + JwtBearer
        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        });

        services.AddAuthorizationBuilder()
            .AddPolicy("Deps.Manage", p => p.RequireRole(AppRoles.Admin, AppRoles.Manager))
            .AddPolicy("Employees.Manage", p => p.RequireRole(AppRoles.Admin, AppRoles.Manager));
            //.AddPolicy("Docs.Read",    p => p.RequireRole(AppRoles.Admin, AppRoles.Manager, AppRoles.User))
            //.AddPolicy("Docs.Manage",  p => p.RequireRole(AppRoles.Admin, AppRoles.Manager));

        authBuilder.AddIdentityCookies();

        authBuilder.AddJwtBearer("JwtAuth", o =>
        {
            o.TokenValidationParameters = new()
            {
                ValidIssuer = jwt.Issuer,
                ValidAudience = jwt.Audience,
                IssuerSigningKey = key, // SymmetricSecurityKey
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            };
        });

        services.AddScoped<ITokenService, TokenService>();
        services.AddHttpContextAccessor();

        return services;
    }
}
