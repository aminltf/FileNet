using FileNet.WebFramework.Contexts;
using FileNet.WebFramework.Services.Abstractions;
using FileNet.WebFramework.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileNet.WebFramework;

public static class DependencyInjection
{
    public static IServiceCollection AddWebFramework(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDocumentService, DocumentService>();
        return services;
    }
}
