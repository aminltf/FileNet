using AutoMapper;
using FileNet.Application.Extensions;
using FileNet.Application.Features.Employees.Dtos;
using FileNet.Domain.Entities;

namespace FileNet.Application.Common.Mappings;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<EmployeeCreateDto, Employee>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Department, o => o.Ignore())
            .ForMember(d => d.Documents, o => o.Ignore())
            .IgnoreAudit();

        CreateMap<EmployeeUpdateDto, Employee>()
            .ForMember(d => d.Department, o => o.Ignore())
            .ForMember(d => d.Documents, o => o.Ignore())
            .IgnoreAudit();

        CreateMap<Employee, EmployeeDto>();
    }
}
