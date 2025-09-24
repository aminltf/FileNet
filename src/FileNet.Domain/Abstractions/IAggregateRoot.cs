namespace FileNet.Domain.Abstractions;

/// <summary>
/// Convenience composition most aggregates will use.
/// </summary>
public interface IAggregateRoot<TKey, TUserId> :
    IHasId<TKey>, IAuditable<TUserId>, ISoftDeletable<TUserId>, IHasConcurrencyToken
    where TKey : notnull, IEquatable<TKey>
    where TUserId : IEquatable<TUserId>;
