using MediatR;

namespace FileNet.Application.Features.Departments.Commands.ChangeDepartmentCode;

public sealed record ChangeDepartmentCodeCommand(Guid Id, string NewCode, Guid? ActorId) : IRequest;
