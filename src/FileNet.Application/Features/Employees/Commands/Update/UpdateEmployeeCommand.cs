using FileNet.Application.Features.Employees.Dtos;
using MediatR;

namespace FileNet.Application.Features.Employees.Commands.Update;

public record UpdateEmployeeCommand(EmployeeUpdateDto Model) : IRequest<bool>;
