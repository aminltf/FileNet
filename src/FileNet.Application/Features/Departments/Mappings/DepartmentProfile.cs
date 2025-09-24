using AutoMapper;
using FileNet.Application.Features.Departments.Dtos;
using FileNet.Domain.Entities;

namespace FileNet.Application.Features.Departments.Mappings;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<DepartmentDto, Department>()
            .ReverseMap();
    }
}
