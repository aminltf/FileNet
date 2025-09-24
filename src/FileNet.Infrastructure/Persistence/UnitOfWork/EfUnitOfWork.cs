using FileNet.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace FileNet.Infrastructure.Persistence.UnitOfWork;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    private readonly ILogger<EfUnitOfWork> _logger;

    private IDbContextTransaction? _currentTx;
    private int _nestedDepth;
    private readonly List<Func<CancellationToken, Task>> _postCommitActions = new(8);

    public bool HasActiveTransaction => _currentTx != null;
    public Guid? CurrentTransactionId => _currentTx?.TransactionId;

    public EfUnitOfWork(AppDbContext db, ILogger<EfUnitOfWork> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);

    public void RegisterPostCommitAction(Func<CancellationToken, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _postCommitActions.Add(callback);
    }

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken ct = default)
    {
        if (_currentTx is null)
        {
            _currentTx = await _db.Database.BeginTransactionAsync(ct);
            _nestedDepth = 0;
            _logger.LogDebug("UoW: Began ROOT transaction {TxId}", _currentTx.TransactionId);
            return new EfUnitOfWorkTransaction(this, _currentTx, isRoot: true, savepoint: null);
        }

        // Nested with savepoints
        _nestedDepth++;
        var sp = $"SP_{_nestedDepth}_{Guid.NewGuid():N}";
        await _currentTx.CreateSavepointAsync(sp, ct);
        _logger.LogDebug("UoW: Created SAVEPOINT {Savepoint} in {TxId}", sp, _currentTx.TransactionId);

        return new EfUnitOfWorkTransaction(this, _currentTx, isRoot: false, savepoint: sp);
    }

    internal void NotifyNestedCompleted()
    {
        _nestedDepth = Math.Max(0, _nestedDepth - 1);
    }

    internal async Task CommitRootAsync(IDbContextTransaction tx, CancellationToken ct)
    {
        try
        {
            await tx.CommitAsync(ct);
            _logger.LogDebug("UoW: COMMIT root transaction {TxId}", tx.TransactionId);

            foreach (var action in _postCommitActions)
            {
                try { await action(ct); }
                catch (Exception ex) { _logger.LogError(ex, "Post-commit action failed."); }
            }
        }
        finally
        {
            await CleanupAsync();
        }
    }

    internal async Task RollbackRootAsync(IDbContextTransaction tx, CancellationToken ct)
    {
        try
        {
            await tx.RollbackAsync(ct);
            _logger.LogWarning("UoW: ROLLBACK root transaction {TxId}", tx.TransactionId);
        }
        finally
        {
            _postCommitActions.Clear();
            await CleanupAsync();
        }
    }

    private async Task CleanupAsync()
    {
        try { await _currentTx!.DisposeAsync(); } catch { /* ignore */ }
        _currentTx = null;
        _nestedDepth = 0;
        _postCommitActions.Clear();
    }

    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken ct = default)
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async token =>
        {
            await using var scope = await BeginTransactionAsync(isolationLevel, token);
            try
            {
                await action(token);
                await SaveChangesAsync(token);
                await scope.CommitAsync(token);
            }
            catch
            {
                await scope.RollbackAsync(token);
                throw;
            }
        }, ct);
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> action,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken ct = default)
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async token =>
        {
            await using var scope = await BeginTransactionAsync(isolationLevel, token);
            try
            {
                var result = await action(token);
                await SaveChangesAsync(token);
                await scope.CommitAsync(token);
                return result;
            }
            catch
            {
                await scope.RollbackAsync(token);
                throw;
            }
        }, ct);
    }
}
