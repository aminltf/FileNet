using FileNet.Domain.Base;
using FileNet.Domain.Enums;

namespace FileNet.Domain.Entities;

public class Employee : AuditableBase
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string NationalCode { get; set; } = null!;
    public Gender Gender { get; set; }

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = default!;

    public ICollection<Document> Documents { get; set; } = new List<Document>();

    public Employee() { }
}
