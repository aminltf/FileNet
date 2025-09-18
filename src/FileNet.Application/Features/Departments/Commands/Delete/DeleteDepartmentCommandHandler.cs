using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Abstractions.UoW;
using FileNet.Application.Common.Commands.Delete;
using FileNet.Domain.Entities;

namespace FileNet.Application.Features.Departments.Commands.Delete;

public class DeleteDepartmentCommandHandler(IRepository<Department> repo, IUnitOfWork uow)
    : DeleteCommandHandlerBase<DeleteDepartmentCommand, Department, IDepartmentRepository>(repo, uow);
