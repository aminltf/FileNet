namespace FileNet.Domain.Events.Departments;

/// <summary>
/// Raised when a department's business code changes.
/// </summary>
public sealed record DepartmentCodeChanged(
    Guid DepartmentId,
    string OldCode,
    string NewCode,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
