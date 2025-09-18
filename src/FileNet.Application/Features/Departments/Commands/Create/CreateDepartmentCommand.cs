using FileNet.Application.Common.Commands.Create;
using FileNet.Application.Features.Departments.Dtos;

namespace FileNet.Application.Features.Departments.Commands.Create;

public record CreateDepartmentCommand(DepartmentCreateDto Model) 
    : CreateCommandBase<DepartmentCreateDto>(Model);
