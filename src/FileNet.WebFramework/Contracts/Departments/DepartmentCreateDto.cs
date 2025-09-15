namespace FileNet.WebFramework.Contracts.Departments;

public class DepartmentCreateDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
