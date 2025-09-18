using FileNet.Application.Common.Abstractions.Commands;
using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Abstractions.UoW;
using FileNet.Domain.Base;
using MediatR;

namespace FileNet.Application.Common.Commands.Delete;

public abstract class DeleteCommandHandlerBase<TCommand, TEntity, TRepository> : IRequestHandler<TCommand, Unit>
    where TCommand : IDeleteCommand
    where TEntity : AuditableBase
    where TRepository : IRepository<TEntity>
{
    protected readonly IRepository<TEntity> _repo;
    protected readonly IUnitOfWork _uow;

    protected DeleteCommandHandlerBase(IRepository<TEntity> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public virtual async Task<Unit> Handle(TCommand request, CancellationToken ct)
    {
        await _repo.DeleteAsync(request.Id, ct);
        await _uow.CommitAsync(ct);
        return Unit.Value;
    }
}
