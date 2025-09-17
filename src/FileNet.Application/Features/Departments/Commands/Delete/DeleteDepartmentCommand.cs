using MediatR;

namespace FileNet.Application.Features.Departments.Commands.Delete;

public record DeleteDepartmentCommand(Guid Id) : IRequest<Unit>;
