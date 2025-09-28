namespace FileNet.Domain.Entities;

public class Employee : Person
{
    public string EmployeeCode { get; set; } = default!;

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = default!;

    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<Dependent> Dependents { get; set; } = new List<Dependent>();

    public Employee() { }
}
