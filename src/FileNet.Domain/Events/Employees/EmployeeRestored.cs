namespace FileNet.Domain.Events.Employees;

public sealed record EmployeeRestored(
    Guid EmployeeId,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
