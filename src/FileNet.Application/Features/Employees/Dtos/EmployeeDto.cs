using FileNet.Domain.Abstractions;
using FileNet.Domain.Enums;

namespace FileNet.Application.Features.Employees.Dtos;

public class EmployeeDto : IEntity
{
    public Guid Id { get; init; }
    public string NationalCode { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public Gender Gender { get; init; }
    public Guid DepartmentId { get; init; }
    public string DepartmentName { get; init; } = default!;
    public int DocumentCount { get; init; }
}
