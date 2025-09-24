namespace FileNet.Application.Features.Departments.Dtos;

public sealed record DepartmentDto(
    Guid Id, string Code, string Name, string? Description,
    DateTimeOffset CreatedOn, DateTimeOffset? ModifiedOn);
