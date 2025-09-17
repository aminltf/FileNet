using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Abstractions.UoW;
using MediatR;

namespace FileNet.Application.Features.Employees.Commands.Delete;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Unit>
{
    private readonly IEmployeeRepository _repo;
    private readonly IUnitOfWork _uow;

    public DeleteEmployeeCommandHandler(IEmployeeRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken ct)
    {
        await _repo.DeleteAsync(request.Id, ct);
        await _uow.CommitAsync(ct);
        return Unit.Value;
    }
}
