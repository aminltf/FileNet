using AutoMapper;
using FileNet.Application.Common.Abstractions.Commands;
using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Abstractions.UoW;
using FileNet.Domain.Base;
using MediatR;

namespace FileNet.Application.Common.Commands.Create;

public abstract class CreateCommandHandlerBase<TCommand, TDto, TEntity, TRepository> : IRequestHandler<TCommand, Guid>
    where TCommand : ICreateCommand<TDto>
    where TEntity : AuditableBase
    where TRepository : IRepository<TEntity>
{
    protected readonly IUnitOfWork _uow;
    protected readonly IRepository<TEntity> _repo;
    protected IMapper _mapper;

    protected CreateCommandHandlerBase(IUnitOfWork uow, IRepository<TEntity> repo, IMapper mapper)
    {
        _uow = uow;
        _repo = repo;
        _mapper = mapper;
    }

    public virtual async Task<Guid> Handle(TCommand request, CancellationToken ct)
    {
        var record = _mapper.Map<TEntity>(request.Model);
        await _repo.CreateAsync(record, ct);
        await _uow.CommitAsync(ct);
        return record.Id;
    }
}
