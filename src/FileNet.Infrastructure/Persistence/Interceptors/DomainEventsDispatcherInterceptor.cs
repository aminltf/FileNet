using FileNet.Domain.Events;
using FileNet.Domain.Primitives;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using FileNet.Application.Abstractions.Messaging;

namespace FileNet.Infrastructure.Persistence.Interceptors;

public sealed class DomainEventsDispatcherInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventDispatcher _dispatcher;

    public DomainEventsDispatcherInterceptor(IDomainEventDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        DispatchDomainEventsAsync(eventData.Context, CancellationToken.None).GetAwaiter().GetResult();
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(eventData.Context, cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(DbContext? context, CancellationToken ct)
    {
        if (context is null) return;

        var entitiesWithEvents = context.ChangeTracker
            .Entries()
            .Select(e => e.Entity)
            .OfType<Entity<Guid>>()
            .ToList();

        var domainEvents = new List<IDomainEvent>(capacity: 32);

        foreach (var entity in entitiesWithEvents)
        {
            if (entity.DomainEvents.Count > 0)
            {
                domainEvents.AddRange(entity.DomainEvents);
                entity.ClearDomainEvents();
            }
        }

        if (domainEvents.Count > 0)
        {
            await _dispatcher.DispatchAsync(domainEvents, ct);
        }
    }
}
