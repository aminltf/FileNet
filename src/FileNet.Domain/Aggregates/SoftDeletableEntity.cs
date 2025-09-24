using FileNet.Domain.Abstractions;

namespace FileNet.Domain.Aggregates;

public abstract class SoftDeletableEntity<TKey, TUserId> :
    AuditableEntity<TKey, TUserId>, ISoftDeletable<TUserId>
    where TKey : notnull, IEquatable<TKey>
    where TUserId : IEquatable<TUserId>
{
    public bool IsDeleted { get; set; }
    public TUserId? DeletedById { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeleteReason { get; set; }

    protected void SoftDelete(TUserId? userId, string? reason = null, DateTimeOffset? nowUtc = null)
    {
        if (IsDeleted) return;
        var ts = nowUtc ?? DateTimeOffset.UtcNow;

        IsDeleted = true;
        DeletedById = userId;
        DeletedOn = ts;
        DeleteReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
    }

    protected void Restore()
    {
        if (!IsDeleted) return;
        IsDeleted = false;
        DeletedById = default;
        DeletedOn = null;
        DeleteReason = null;
    }

    public void EnsureDeletionInvariant()
    {
        if (IsDeleted && DeletedOn is null)
            DeletedOn = DateTimeOffset.UtcNow;
        if (!IsDeleted)
        {
            DeletedOn = null;
            DeletedById = default;
            DeleteReason = null;
        }
    }
}
