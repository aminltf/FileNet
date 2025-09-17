using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Domain.Base;
using FileNet.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FileNet.Infrastructure.Persistence.Repositories;

public class EfRepository<TEntity> : IRepository<TEntity> 
    where TEntity : AuditableBase
{
    private readonly AppDbContext _db;
    private readonly DbSet<TEntity> _set;

    public EfRepository(AppDbContext db)
    {
        _db = db;
        _set = _db.Set<TEntity>();
    }

    public Task CreateAsync(TEntity entity, CancellationToken ct)
        => _set.AddAsync(entity, ct).AsTask();

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct)
        => await _set.AsNoTracking().ToListAsync(ct);

    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct)
        => _set.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task UpdateAsync(TEntity entity, CancellationToken ct)
    {
        _db.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await _set.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is null) return;
        _set.Remove(entity);
    }
}
