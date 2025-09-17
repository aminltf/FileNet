using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Abstractions.UoW;
using MediatR;

namespace FileNet.Application.Features.Departments.Commands.Delete;

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Unit>
{
    private readonly IDepartmentRepository _repo;
    private readonly IUnitOfWork _uow;

    public DeleteDepartmentCommandHandler(IDepartmentRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteDepartmentCommand request, CancellationToken ct)
    {
        await _repo.DeleteAsync(request.Id, ct);
        await _uow.CommitAsync(ct);
        return Unit.Value;
    }
}
