namespace FileNet.Domain.Events;

public abstract record DomainEventBase(DateTimeOffset OccurredOn) : IDomainEvent
{
    protected DomainEventBase() : this(DateTimeOffset.UtcNow) { }
}
