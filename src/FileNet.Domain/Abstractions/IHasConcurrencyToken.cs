namespace FileNet.Domain.Abstractions;

/// <summary>
/// Optimistic concurrency token (rowversion on SQL Server, xmin on PostgreSQL, etc.)
/// </summary>
public interface IHasConcurrencyToken
{
    byte[]? ConcurrencyToken { get; set; }
}
