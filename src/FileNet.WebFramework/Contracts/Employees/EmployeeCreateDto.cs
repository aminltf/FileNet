using System.ComponentModel.DataAnnotations;
using FileNet.WebFramework.Enums;

namespace FileNet.WebFramework.Contracts.Employees;

public class EmployeeCreateDto
{
    [Required, StringLength(10)]
    public string NationalCode { get; set; } = default!;

    [Required, StringLength(100)]
    public string FirstName { get; set; } = default!;

    [Required, StringLength(100)]
    public string LastName { get; set; } = default!;

    [Required]
    public Gender Gender { get; set; }

    [Required]
    public Guid DepartmentId { get; set; }
}
