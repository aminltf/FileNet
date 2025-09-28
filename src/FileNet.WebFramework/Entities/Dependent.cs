using FileNet.WebFramework.Enums;

namespace FileNet.WebFramework.Entities;

public class Dependent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string NationalCode { get; set; } = default!;

    public Gender Gender { get; set; }
    public Relation Relation { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }

    public Dependent()
    {
        
    }
}
