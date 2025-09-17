using FluentValidation;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace FileNet.Application.Extensions.DependencyInjection;

public static class ApplicationConfigurationExtensions
{
    public static IServiceCollection ApplicationServices(this IServiceCollection services)
    {
        // Register Validations
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
