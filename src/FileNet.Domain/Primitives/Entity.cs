using FileNet.Domain.Abstractions;
using FileNet.Domain.Events;

namespace FileNet.Domain.Primitives;

public abstract class Entity<TKey> :
    IHasId<TKey>, IHasConcurrencyToken, IEquatable<Entity<TKey>>
    where TKey : notnull, IEquatable<TKey>
{
    public TKey Id { get; protected set; } = default!;
    public byte[]? ConcurrencyToken { get; set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);
        _domainEvents.Add(@event);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    public override bool Equals(object? obj) => Equals(obj as Entity<TKey>);

    public bool Equals(Entity<TKey>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        if (EqualityComparer<TKey>.Default.Equals(Id, default!)) return false;
        return EqualityComparer<TKey>.Default.Equals(Id, other.Id);
    }

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    protected Entity() { } // For EF
}
