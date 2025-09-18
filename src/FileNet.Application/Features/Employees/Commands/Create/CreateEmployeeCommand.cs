using FileNet.Application.Common.Commands.Create;
using FileNet.Application.Features.Employees.Dtos;

namespace FileNet.Application.Features.Employees.Commands.Create;

public record CreateEmployeeCommand(EmployeeCreateDto Model) : CreateCommandBase<EmployeeCreateDto>(Model);
