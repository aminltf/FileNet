using FileNet.Application.Extensions;
using FileNet.Domain.Enums;
using FluentValidation;

namespace FileNet.Application.Features.Employees.Dtos;

public class EmployeeCreateDto
{
    public string NationalCode { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public Gender Gender { get; set; }
    public Guid DepartmentId { get; set; }
}

public class EmployeeCreateDtoValidator : AbstractValidator<EmployeeCreateDto>
{
    public EmployeeCreateDtoValidator()
    {
        RuleFor(x => x.NationalCode)
            .NotEmpty().WithMessage("National code is required.")
            .Must(FluentValidationExtensions.IsValidIranianNationalCode)
            .WithMessage("National code is not valid.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must be a max of 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must be a max of 50 characters.");

        RuleFor(x => x.Gender)
           .IsInEnum().WithMessage("Gender is not valid.");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Department ID is required.");
    }
}
