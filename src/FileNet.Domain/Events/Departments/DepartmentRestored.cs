namespace FileNet.Domain.Events.Departments;

/// <summary>
/// Raised when a previously soft-deleted department is restored.
/// </summary>
public sealed record DepartmentRestored(
    Guid DepartmentId,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
