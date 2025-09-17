using MediatR;

namespace FileNet.Application.Features.Employees.Commands.Delete;

public record DeleteEmployeeCommand(Guid Id) : IRequest<Unit>;
