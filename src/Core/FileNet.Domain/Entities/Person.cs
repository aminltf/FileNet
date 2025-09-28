using FileNet.Domain.Common.Base;
using FileNet.Domain.Enums;

namespace FileNet.Domain.Entities;

public abstract class Person : AuditableBase
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string NationalCode { get; set; } = null!;
    public Gender Gender { get; set; }
}
