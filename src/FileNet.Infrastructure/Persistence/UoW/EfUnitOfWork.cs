using FileNet.Application.Common.Abstractions.UoW;
using Microsoft.EntityFrameworkCore.Storage;
using FileNet.Infrastructure.Persistence.Contexts;

namespace FileNet.Infrastructure.Persistence.UoW;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _tx;

    public EfUnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public Task<int> CommitAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_tx is not null) return; // already in a transaction
        _tx = await _context.Database.BeginTransactionAsync(ct).ConfigureAwait(false);
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_tx is null)
        {
            // no explicit transaction, just save changes
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            return;
        }

        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        await _tx.CommitAsync(ct).ConfigureAwait(false);
        await _tx.DisposeAsync().ConfigureAwait(false);
        _tx = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_tx is not null)
        {
            await _tx.RollbackAsync(ct).ConfigureAwait(false);
            await _tx.DisposeAsync().ConfigureAwait(false);
            _tx = null;
        }

        // discard pending tracked changes
        _context.ChangeTracker.Clear();
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _tx?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_tx is not null) await _tx.DisposeAsync().ConfigureAwait(false);
    }
}
