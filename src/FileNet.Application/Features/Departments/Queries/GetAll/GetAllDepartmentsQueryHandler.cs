using AutoMapper;
using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Queryable;
using FileNet.Application.Features.Departments.Dtos;
using MediatR;

namespace FileNet.Application.Features.Departments.Queries.GetAll;

public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, PageResponse<DepartmentDto>>
{
    private readonly IDepartmentRepository _repo;
    private readonly IMapper _mapper;

    public GetAllDepartmentsQueryHandler(IDepartmentRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<PageResponse<DepartmentDto>> Handle(GetAllDepartmentsQuery request, CancellationToken ct)
    {
        var deps = await _repo.GetPagedAsync(
            request.Page,
            request.Search,
            request.Sort,
            ct
        );

        var items = _mapper.Map<List<DepartmentDto>>(deps.Items);

        return new PageResponse<DepartmentDto>
        {
            Items = items,
            TotalCount = deps.TotalCount,
            PageNumber = deps.PageNumber,
            PageSize = deps.PageSize
        };
    }
}
