using FileNet.Application.Common.Commands.Update;
using FileNet.Application.Features.Employees.Dtos;

namespace FileNet.Application.Features.Employees.Commands.Update;

public record UpdateEmployeeCommand(EmployeeUpdateDto Model) : UpdateCommandBase<EmployeeUpdateDto>(Model);
