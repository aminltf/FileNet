using MediatR;

namespace FileNet.Application.Features.Departments.Commands.RenameDepartment;

public sealed record RenameDepartmentCommand(Guid Id, string Name, string? Description, Guid? ActorId) : IRequest;
