namespace FileNet.Domain.Abstractions;

/// <summary>
/// Strongly-typed identity for entities/aggregates.
/// EF can set a private setter; keep public API read-only.
/// </summary>
public interface IHasId<out TKey>
    where TKey : notnull, IEquatable<TKey>
{
    TKey Id { get; }
}
