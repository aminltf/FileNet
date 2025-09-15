using FileNet.WebFramework.Enums;

namespace FileNet.WebFramework.Contracts.Employees;

public class EmployeeDto
{
    public Guid Id { get; init; }
    public string NationalCode { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public Gender Gender { get; init; }
    public int DocumentCount { get; init; }
}
