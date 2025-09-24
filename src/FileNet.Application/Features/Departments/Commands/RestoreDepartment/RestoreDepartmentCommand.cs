using MediatR;

namespace FileNet.Application.Features.Departments.Commands.RestoreDepartment;

public sealed record RestoreDepartmentCommand(Guid Id, Guid? ActorId) : IRequest;
