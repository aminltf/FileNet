namespace FileNet.Domain.Events.Documents;

/// <summary>
/// Raised after a document is uploaded.
/// </summary>
public sealed record DocumentUploaded(
    Guid DocumentId,
    Guid EmployeeId,
    short DocumentCategoryCode,
    string FileName,
    string ContentType,
    long SizeBytes,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
