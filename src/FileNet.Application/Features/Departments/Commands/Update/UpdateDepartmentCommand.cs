using FileNet.Application.Features.Departments.Dtos;
using MediatR;

namespace FileNet.Application.Features.Departments.Commands.Update;

public record UpdateDepartmentCommand(DepartmentUpdateDto Model) : IRequest<bool>;
