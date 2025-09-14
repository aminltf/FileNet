using System.ComponentModel.DataAnnotations;

namespace FileNet.WebFramework.Contracts.Employees;

public class EmployeeCreateDto
{
    [Required, StringLength(10)]
    public string NationalCode { get; set; } = default!;

    [Required, StringLength(100)]
    public string FirstName { get; set; } = default!;

    [Required, StringLength(100)]
    public string LastName { get; set; } = default!;
}
