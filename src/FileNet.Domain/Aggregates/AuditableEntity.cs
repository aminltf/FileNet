using FileNet.Domain.Abstractions;
using FileNet.Domain.Primitives;

namespace FileNet.Domain.Aggregates;

public abstract class AuditableEntity<TKey, TUserId> :
    Entity<TKey>, IAuditable<TUserId>
    where TKey : notnull, IEquatable<TKey>
    where TUserId : IEquatable<TUserId>
{
    public TUserId? CreatedById { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public TUserId? ModifiedById { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }

    protected void MarkCreated(TUserId? userId, DateTimeOffset? nowUtc = null)
    {
        var ts = nowUtc ?? DateTimeOffset.UtcNow;
        CreatedById = userId;
        CreatedOn = ts;
        ModifiedById = default;
        ModifiedOn = null;
    }

    protected void MarkModified(TUserId? userId, DateTimeOffset? nowUtc = null)
    {
        var ts = nowUtc ?? DateTimeOffset.UtcNow;
        ModifiedById = userId;
        ModifiedOn = ts;
    }
}
