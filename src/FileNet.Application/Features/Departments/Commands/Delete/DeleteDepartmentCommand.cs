using FileNet.Application.Common.Commands.Delete;

namespace FileNet.Application.Features.Departments.Commands.Delete;

public record DeleteDepartmentCommand(Guid Id) : DeleteCommandBase(Id);
