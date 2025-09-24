using FileNet.Domain.Abstractions;

namespace FileNet.Domain.Aggregates;

public abstract class AggregateRoot<TKey, TUserId> :
    SoftDeletableEntity<TKey, TUserId>, IAggregateRoot<TKey, TUserId>
    where TKey : notnull, IEquatable<TKey>
    where TUserId : IEquatable<TUserId>
{
    protected void CheckInvariants()
    {
        EnsureDeletionInvariant();
    }
}
