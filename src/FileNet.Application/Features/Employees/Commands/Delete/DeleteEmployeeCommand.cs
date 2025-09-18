using FileNet.Application.Common.Commands.Delete;

namespace FileNet.Application.Features.Employees.Commands.Delete;

public record DeleteEmployeeCommand(Guid Id) : DeleteCommandBase(Id);
