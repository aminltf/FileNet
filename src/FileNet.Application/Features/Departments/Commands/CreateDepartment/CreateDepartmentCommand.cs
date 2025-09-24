using MediatR;

namespace FileNet.Application.Features.Departments.Commands.CreateDepartment;

public sealed record CreateDepartmentCommand(
    string Code, string Name, string? Description, Guid? ActorId
) : IRequest<Guid>;
