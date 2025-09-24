namespace FileNet.Domain.Events.Departments;

/// <summary>
/// Raised after a department is successfully created.
/// </summary>
public sealed record DepartmentCreated(
    Guid DepartmentId,
    string Code,
    string Name,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
