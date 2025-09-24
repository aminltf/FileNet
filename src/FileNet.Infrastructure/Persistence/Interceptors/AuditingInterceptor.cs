using FileNet.Domain.Aggregates;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace FileNet.Infrastructure.Persistence.Interceptors;

public sealed class AuditingInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        Stamp(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Stamp(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void Stamp(DbContext? context)
    {
        if (context is null) return;

        var now = DateTimeOffset.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added ||
                entry.State == EntityState.Modified ||
                entry.State == EntityState.Deleted)
            {
                // CreatedOn / ModifiedOn
                if (entry.Entity is AuditableEntity<Guid, Guid> aud)
                {
                    if (entry.State == EntityState.Added && aud.CreatedOn == default)
                        aud.CreatedOn = now;

                    if (entry.State == EntityState.Modified)
                        aud.ModifiedOn = now;
                }

                // Enforce soft-delete invariant
                if (entry.Entity is SoftDeletableEntity<Guid, Guid> soft)
                {
                    soft.EnsureDeletionInvariant();
                }
            }
        }
    }
}
