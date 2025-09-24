using FileNet.Application.Abstractions.Persistence.Repositories;
using FileNet.Application.Abstractions.Persistence;
using FileNet.Infrastructure.Persistence;
using FileNet.Infrastructure.Persistence.Interceptors;
using FileNet.Infrastructure.Persistence.Repositories;
using FileNet.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FileNet.Application.Abstractions.Messaging;
using FileNet.Infrastructure.Messaging;

namespace FileNet.Infrastructure.Extensions.DependencyInjection;

public static class InfrastructureConfigurationExtensions
{
    public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDb)
    {
        // Interceptors
        services.AddScoped<AuditingInterceptor>();
        services.AddScoped<DomainEventsDispatcherInterceptor>();

        // DbContext + Interceptors
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            configureDb(options);

            options.AddInterceptors(
                sp.GetRequiredService<AuditingInterceptor>(),
                sp.GetRequiredService<DomainEventsDispatcherInterceptor>());
        });

        // UoW
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

        // Repositories
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();

        return services;
    }
}
