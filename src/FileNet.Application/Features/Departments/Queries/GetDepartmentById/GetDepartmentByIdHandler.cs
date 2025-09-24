using AutoMapper;
using FileNet.Application.Abstractions.Persistence.Repositories;
using FileNet.Application.Features.Departments.Dtos;
using MediatR;

namespace FileNet.Application.Features.Departments.Queries.GetDepartmentById;

public sealed class GetDepartmentByIdHandler(
    IDepartmentRepository departments,
    IMapper mapper
) : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto>
{
    public async Task<DepartmentDto> Handle(GetDepartmentByIdQuery req, CancellationToken ct)
    {
        var response = await departments.GetByIdAsync(req.Id, false, ct);

        if (response is null)
            throw new KeyNotFoundException("Department not found.");

        return mapper.Map<DepartmentDto>(response);
    }
}
