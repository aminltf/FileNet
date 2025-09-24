using FileNet.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace FileNet.Infrastructure.Persistence.UnitOfWork;

internal sealed class EfUnitOfWorkTransaction : IUnitOfWorkTransaction
{
    private readonly EfUnitOfWork _uow;
    private readonly IDbContextTransaction _efTx;
    private readonly string? _savepoint;
    private bool _completed;
    public Guid Id { get; }
    public bool IsRoot { get; }

    public EfUnitOfWorkTransaction(EfUnitOfWork uow, IDbContextTransaction efTx, bool isRoot, string? savepoint)
    {
        _uow = uow;
        _efTx = efTx;
        IsRoot = isRoot;
        _savepoint = savepoint;
        Id = isRoot ? efTx.TransactionId : Guid.NewGuid();
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_completed) return;

        if (IsRoot)
            await _uow.CommitRootAsync(_efTx, ct);
        else
        {
            if (!string.IsNullOrEmpty(_savepoint))
                await _efTx.ReleaseSavepointAsync(_savepoint!, ct);
            _uow.NotifyNestedCompleted();
        }

        _completed = true;
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_completed) return;

        if (IsRoot)
            await _uow.RollbackRootAsync(_efTx, ct);
        else
        {
            if (!string.IsNullOrEmpty(_savepoint))
                await _efTx.RollbackToSavepointAsync(_savepoint!, ct);
            if (!string.IsNullOrEmpty(_savepoint))
                await _efTx.ReleaseSavepointAsync(_savepoint!, ct);

            _uow.NotifyNestedCompleted();
        }

        _completed = true;
    }

    public Task CreateSavepointAsync(string name, CancellationToken ct = default) =>
        _efTx.CreateSavepointAsync(name, ct);

    public Task RollbackToSavepointAsync(string name, CancellationToken ct = default) =>
        _efTx.RollbackToSavepointAsync(name, ct);

    public Task ReleaseSavepointAsync(string name, CancellationToken ct = default) =>
        _efTx.ReleaseSavepointAsync(name, ct);

    public async ValueTask DisposeAsync()
    {
        if (!_completed)
        {
            try { await RollbackAsync(); } catch { /* swallow */ }
        }
    }
}
