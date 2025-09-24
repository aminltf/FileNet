namespace FileNet.Domain.Events.Employees;

/// <summary>
/// Raised when first/last name changes.
/// </summary>
public sealed record EmployeeRenamed(
    Guid EmployeeId,
    string OldFirstName,
    string OldLastName,
    string NewFirstName,
    string NewLastName,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
