namespace FileNet.Domain.Events.Departments;

/// <summary>
/// Raised when a department's name/description is updated.
/// </summary>
public sealed record DepartmentRenamed(
    Guid DepartmentId,
    string OldName,
    string NewName,
    string? OldDescription,
    string? NewDescription,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
