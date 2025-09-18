using FileNet.Application.Common.Commands.Update;
using FileNet.Application.Features.Departments.Dtos;

namespace FileNet.Application.Features.Departments.Commands.Update;

public record UpdateDepartmentCommand(DepartmentUpdateDto Model) : UpdateCommandBase<DepartmentUpdateDto>(Model);
