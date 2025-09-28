using FileNet.Domain.Common.Base;
using FileNet.Domain.Enums;

namespace FileNet.Domain.Entities;

public class Dependent : Person
{
    public Relation Relation { get; set; }

    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;

    public Dependent() { }
}
