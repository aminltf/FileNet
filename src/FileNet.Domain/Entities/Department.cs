using FileNet.Domain.Base;

namespace FileNet.Domain.Entities;

public class Department : AuditableBase
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public Department() { }
}
