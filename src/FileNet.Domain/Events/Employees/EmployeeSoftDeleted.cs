namespace FileNet.Domain.Events.Employees;

public sealed record EmployeeSoftDeleted(
    Guid EmployeeId,
    string? Reason,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
