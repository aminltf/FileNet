using AutoMapper;
using FileNet.Application.Common.Abstractions.Commands;
using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Abstractions.UoW;
using FileNet.Domain.Abstractions;
using FileNet.Domain.Base;
using MediatR;

namespace FileNet.Application.Common.Commands.Update;

public abstract class UpdateCommandHandlerBase<TCommand, TUpdateDto, TEntity, TRepository> : IRequestHandler<TCommand, bool>
    where TCommand : IUpdateCommand<TUpdateDto>
    where TUpdateDto : IEntity
    where TEntity : AuditableBase
    where TRepository : IRepository<TEntity>
{
    protected readonly IRepository<TEntity> _repo;
    protected readonly IUnitOfWork _uow;
    protected readonly IMapper _mapper;

    protected UpdateCommandHandlerBase(IRepository<TEntity> repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo;
        _uow = uow;
        _mapper = mapper;
    }

    public virtual async Task<bool> Handle(TCommand request, CancellationToken ct)
    {
        var record = await _repo.GetByIdAsync(request.Model.Id, ct);
        if (record is null)
            return false;

        _mapper.Map(request.Model, record);
        await _uow.CommitAsync(ct);
        return true;
    }
}
