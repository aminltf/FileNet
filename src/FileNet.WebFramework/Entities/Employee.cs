using FileNet.WebFramework.Enums;

namespace FileNet.WebFramework.Entities;

public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string NationalCode { get; set; } = null!;
    public Gender Gender { get; set; }

    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;

    public ICollection<Document> Documents { get; set; } = new List<Document>();

    public Employee()
    {

    }
}
