namespace FileNet.Application.Common.Abstractions.UoW;

public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
    Task<int> CommitAsync(CancellationToken ct = default);

    Task BeginTransactionAsync(CancellationToken ct = default);

    Task CommitTransactionAsync(CancellationToken ct = default);

    Task RollbackTransactionAsync(CancellationToken ct = default);
}
