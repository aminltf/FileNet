namespace FileNet.Domain.Events.Documents;

public sealed record DocumentFileReplaced(
    Guid DocumentId,
    string OldFileName,
    string NewFileName,
    string OldContentType,
    string NewContentType,
    long OldSizeBytes,
    long NewSizeBytes,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
