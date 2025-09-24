namespace FileNet.Domain.Events.Documents;

/// <summary>
/// Raised when the document title changes.
/// </summary>
public sealed record DocumentRetitled(
    Guid DocumentId,
    string OldTitle,
    string NewTitle,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
