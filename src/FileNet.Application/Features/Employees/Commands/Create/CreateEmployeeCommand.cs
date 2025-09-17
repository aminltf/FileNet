using FileNet.Application.Features.Employees.Dtos;
using MediatR;

namespace FileNet.Application.Features.Employees.Commands.Create;

public record CreateEmployeeCommand(EmployeeCreateDto Model) : IRequest<Guid>;
