using AutoMapper;
using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Abstractions.UoW;
using FileNet.Application.Common.Commands.Update;
using FileNet.Application.Features.Departments.Dtos;
using FileNet.Domain.Entities;

namespace FileNet.Application.Features.Departments.Commands.Update;

public class UpdateDepartmentCommandHandler(IRepository<Department> repo, IUnitOfWork uow, IMapper mapper) 
    : UpdateCommandHandlerBase<UpdateDepartmentCommand, DepartmentUpdateDto, Department, IDepartmentRepository>(repo, uow, mapper);
