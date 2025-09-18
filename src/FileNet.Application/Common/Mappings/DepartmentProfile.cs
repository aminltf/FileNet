using AutoMapper;
using FileNet.Application.Extensions;
using FileNet.Application.Features.Departments.Dtos;
using FileNet.Domain.Entities;

namespace FileNet.Application.Common.Mappings;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<DepartmentCreateDto, Department>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Employees, o => o.Ignore())
            .IgnoreAudit();

        CreateMap<DepartmentUpdateDto, Department>()
            .ForMember(d => d.Employees, o => o.Ignore())
            .IgnoreAudit();

        CreateMap<Department, DepartmentDto>();
    }
}
