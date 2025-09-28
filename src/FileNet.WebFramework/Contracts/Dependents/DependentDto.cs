using FileNet.WebFramework.Enums;

namespace FileNet.WebFramework.Contracts.Dependents;

public class DependentDto
{
    public Guid Id { get; init; }
    public string NationalCode { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public Gender Gender { get; init; }
    public Relation Relation { get; init; }
    public Guid EmployeeId { get; init; }
    public string EmployeeFullName { get; init; } = default!;
}
