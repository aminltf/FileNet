using FileNet.Application.Common.Queryable;
using FileNet.Application.Features.Departments.Dtos;
using MediatR;

namespace FileNet.Application.Features.Departments.Queries.GetAll;

public record GetAllDepartmentsQuery : IRequest<PageResponse<DepartmentDto>>
{
    public PageRequest Page { get; init; } = new();
    public SearchRequest Search { get; init; } = new();
    public SortOptions Sort { get; init; } = new();
}
