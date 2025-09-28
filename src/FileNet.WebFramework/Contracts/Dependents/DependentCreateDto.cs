using FileNet.WebFramework.Enums;
using System.ComponentModel.DataAnnotations;

namespace FileNet.WebFramework.Contracts.Dependents;

public class DependentCreateDto
{
    [Required]
    public Guid EmployeeId { get; set; }

    [Required, StringLength(10)]
    public string NationalCode { get; set; } = default!;

    [Required, StringLength(100)]
    public string FirstName { get; set; } = default!;

    [Required, StringLength(100)]
    public string LastName { get; set; } = default!;

    [Required]
    public Gender Gender { get; set; }

    [Required]
    public Relation Relation { get; set; }
}
