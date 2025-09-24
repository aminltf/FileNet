namespace FileNet.Domain.Events.Documents;

public sealed record DocumentRestored(
    Guid DocumentId,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
