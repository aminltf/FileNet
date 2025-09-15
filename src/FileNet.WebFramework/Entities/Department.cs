namespace FileNet.WebFramework.Entities;

public class Department
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public Department()
    {
        
    }
}
