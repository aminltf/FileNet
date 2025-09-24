using Microsoft.Extensions.DependencyInjection;

namespace FileNet.Application.Extensions.DependencyInjection;

public static class ApplicationConfigurationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationConfigurationExtensions).Assembly;

        // Handlers (Commands/Queries)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // AutoMapper Profiles
        services.AddAutoMapper(assembly);

        // services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
