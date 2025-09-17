using FileNet.Application.Features.Departments.Dtos;
using MediatR;

namespace FileNet.Application.Features.Departments.Commands.Create;

public record CreateDepartmentCommand(DepartmentCreateDto Model) : IRequest<Guid>;
