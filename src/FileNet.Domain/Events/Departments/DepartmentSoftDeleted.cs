namespace FileNet.Domain.Events.Departments;

/// <summary>
/// Raised when department is soft-deleted.
/// </summary>
public sealed record DepartmentSoftDeleted(
    Guid DepartmentId,
    string? Reason,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
