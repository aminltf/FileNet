namespace FileNet.Domain.Events.Employees;

// <summary>
/// Raised when the employee's gender changes from one code to another.
/// </summary>
public sealed record EmployeeGenderChanged(
    Guid EmployeeId,
    short OldGender,   // store enum underlying for simpler consumers
    short NewGender,
    Guid? ActorId
) : UserActionDomainEventBase(ActorId);
