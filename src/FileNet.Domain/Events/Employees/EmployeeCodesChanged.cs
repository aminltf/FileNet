namespace FileNet.Domain.Events.Employees;

/// <summary>
/// Raised when national/employee codes change.
/// </summary>
public sealed record EmployeeCodesChanged(
    Guid EmployeeId,
    string OldNationalCode,
    string NewNationalCode,
    string OldEmployeeCode,
    string NewEmployeeCode,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
