using FluentValidation;

namespace FileNet.Application.Features.Departments.Dtos;

public class DepartmentUpdateDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class DepartmentUpdateDtoValidator : AbstractValidator<DepartmentUpdateDto>
{
    public DepartmentUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Department ID is required.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Department code is required.")
            .MaximumLength(32).WithMessage("Department code must be a max of 10 characters.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Department name is required.")
            .MaximumLength(128).WithMessage("Department name must be a max of 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Department description is required.")
            .MaximumLength(512).WithMessage("Department description must be a max of 500 characters.");
    }
}
