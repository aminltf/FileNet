namespace FileNet.Domain.Events.Documents;

public sealed record DocumentSoftDeleted(
    Guid DocumentId,
    string? Reason,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
