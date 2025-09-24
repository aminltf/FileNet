using FileNet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileNet.Infrastructure.Extensions.DependencyInjection;

public static class InfrastructureConfigurationExtensions
{
    public static IServiceCollection InfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        return services;
    }
}
