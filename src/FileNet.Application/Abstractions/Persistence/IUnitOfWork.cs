using System.Transactions;

namespace FileNet.Application.Abstractions.Persistence;

/// <summary>
/// Unit of Work for coordinating SaveChanges and transactions.
/// Supports nested transactions (savepoints), execution-strategy retries, and post-commit actions.
/// </summary>
public interface IUnitOfWork
{
    bool HasActiveTransaction { get; }
    Guid? CurrentTransactionId { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task<IUnitOfWorkTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken ct = default);

    /// <summary>Register callbacks that run only after the ROOT transaction commits successfully.</summary>
    void RegisterPostCommitAction(Func<CancellationToken, Task> callback);

    Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken ct = default);

    Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> action,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken ct = default);
}

public interface IUnitOfWorkTransaction : IAsyncDisposable
{
    Guid Id { get; }
    bool IsRoot { get; }

    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);

    Task CreateSavepointAsync(string name, CancellationToken ct = default);
    Task RollbackToSavepointAsync(string name, CancellationToken ct = default);
    Task ReleaseSavepointAsync(string name, CancellationToken ct = default);
}
