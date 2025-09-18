using AutoMapper;
using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Abstractions.UoW;
using FileNet.Application.Common.Commands.Create;
using FileNet.Application.Features.Employees.Dtos;
using FileNet.Domain.Entities;

namespace FileNet.Application.Features.Employees.Commands.Create;

public class CreateEmployeeCommandHandler(IUnitOfWork uow, IRepository<Employee> repo, IMapper mapper)
    : CreateCommandHandlerBase<CreateEmployeeCommand, EmployeeCreateDto, Employee, IEmployeeRepository>(uow, repo, mapper);
