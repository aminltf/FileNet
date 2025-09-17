using FileNet.Domain.Base;

namespace FileNet.Application.Common.Abstractions.Repositories;

public interface IRepository<TEntity> where TEntity : AuditableBase
{
    Task CreateAsync(TEntity entity, CancellationToken ct);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct);
    Task UpdateAsync(TEntity entity, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
