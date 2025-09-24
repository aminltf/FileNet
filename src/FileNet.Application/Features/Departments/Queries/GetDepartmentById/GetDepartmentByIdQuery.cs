using FileNet.Application.Features.Departments.Dtos;
using MediatR;

namespace FileNet.Application.Features.Departments.Queries.GetDepartmentById;

public sealed record GetDepartmentByIdQuery(Guid Id) : IRequest<DepartmentDto>;
