namespace FileNet.Domain.Events.Employees;

/// <summary>
/// Raised after an employee is created.
/// </summary>
public sealed record EmployeeCreated(
    Guid EmployeeId,
    Guid DepartmentId,
    string NationalCode,
    string EmployeeCode,
    short GenderCode,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
