namespace FileNet.Domain.Events.Employees;

/// <summary>
/// Raised when employee moves to a different department.
/// </summary>
public sealed record EmployeeTransferred(
    Guid EmployeeId,
    Guid OldDepartmentId,
    Guid NewDepartmentId,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
