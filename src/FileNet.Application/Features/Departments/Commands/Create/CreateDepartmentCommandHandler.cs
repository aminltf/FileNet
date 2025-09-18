using AutoMapper;
using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Abstractions.UoW;
using FileNet.Application.Common.Commands.Create;
using FileNet.Application.Features.Departments.Dtos;
using FileNet.Domain.Entities;

namespace FileNet.Application.Features.Departments.Commands.Create;

public class CreateDepartmentCommandHandler(IUnitOfWork uow, IRepository<Department> repo, IMapper mapper) 
    : CreateCommandHandlerBase<CreateDepartmentCommand, DepartmentCreateDto, Department, IDepartmentRepository>(uow, repo, mapper);
