using AutoMapper;
using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Abstractions.UoW;
using FileNet.Application.Common.Commands.Update;
using FileNet.Application.Features.Employees.Dtos;
using FileNet.Domain.Entities;

namespace FileNet.Application.Features.Employees.Commands.Update;

public class UpdateEmployeeCommandHandler(IRepository<Employee> repo, IUnitOfWork uow, IMapper mapper) 
    : UpdateCommandHandlerBase<UpdateEmployeeCommand, EmployeeUpdateDto, Employee, IEmployeeRepository>(repo, uow, mapper);