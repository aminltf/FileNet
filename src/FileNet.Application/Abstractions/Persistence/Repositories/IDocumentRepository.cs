using FileNet.Domain.Entities;

namespace FileNet.Application.Abstractions.Persistence.Repositories;

/// <summary>
/// Thin repository for Document aggregate: load, add, and per-employee filename uniqueness.
/// </summary>
public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default);
    Task AddAsync(Document entity, CancellationToken ct = default);

    /// <summary>Does an active document with the same file name exist for the employee (excluding self)?</summary>
    Task<bool> ExistsActiveFileNameForEmployeeAsync(Guid employeeId, string fileName, Guid? excludingId = null, CancellationToken ct = default);
}
