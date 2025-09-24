namespace FileNet.Domain.Events;

public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}
