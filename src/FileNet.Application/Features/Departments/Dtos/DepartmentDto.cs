namespace FileNet.Application.Features.Departments.Dtos;

public class DepartmentDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public int EmployeeCount { get; init; }
}
