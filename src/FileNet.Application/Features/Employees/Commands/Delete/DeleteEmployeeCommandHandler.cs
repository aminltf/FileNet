using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Abstractions.UoW;
using FileNet.Application.Common.Commands.Delete;
using FileNet.Domain.Entities;

namespace FileNet.Application.Features.Employees.Commands.Delete;

public class DeleteEmployeeCommandHandler(IRepository<Employee> repo, IUnitOfWork uow) 
    : DeleteCommandHandlerBase<DeleteEmployeeCommand, Employee, IEmployeeRepository>(repo, uow);
