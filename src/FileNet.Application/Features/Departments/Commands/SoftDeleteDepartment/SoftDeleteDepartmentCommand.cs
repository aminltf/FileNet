using MediatR;

namespace FileNet.Application.Features.Departments.Commands.SoftDeleteDepartment;

public sealed record SoftDeleteDepartmentCommand(Guid Id, string? Reason, Guid? ActorId) : IRequest;
