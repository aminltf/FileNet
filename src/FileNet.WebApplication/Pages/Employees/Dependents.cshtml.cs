using System.ComponentModel.DataAnnotations;
using FileNet.WebFramework.Contracts.Dependents;
using FileNet.WebFramework.Contracts.Documents;
using FileNet.WebFramework.Contracts.Employees;
using FileNet.WebFramework.Enums;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileNet.WebApplication.Pages.Dependents;

public class CreateModel(
    IEmployeeService empSvc,
    IDependentService depSvc) : PageModel
{
    private readonly IEmployeeService _empSvc = empSvc;
    private readonly IDependentService _depSvc = depSvc;

    [FromRoute(Name = "id")] public Guid EmployeeId { get; set; }

    public EmployeeDto? Employee { get; private set; }
    public IReadOnlyList<DependentDto> Dependents { get; private set; } = [];

    [BindProperty] public CreateDependentInput Input { get; set; } = new();

    [TempData] public string? Success { get; set; }
    [TempData] public string? Error { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        Employee = await _empSvc.GetByIdAsync(EmployeeId, ct);
        if (Employee is null) return NotFound();

        Dependents = await _depSvc.GetByEmployeeAsync(EmployeeId, ct);
        return Page();
    }

    public class CreateDependentInput
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
        public Relation Relation { get; set; }
    }
}
