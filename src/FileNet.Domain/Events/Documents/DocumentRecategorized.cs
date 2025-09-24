namespace FileNet.Domain.Events.Documents;

public sealed record DocumentRecategorized(
    Guid DocumentId,
    short OldCategoryCode,
    short NewCategoryCode,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
