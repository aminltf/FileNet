using FileNet.Domain.Entities;

namespace FileNet.Application.Abstractions.Persistence.Repositories;

/// <summary>
/// Thin repository focused on Department aggregate lifecycle:
/// load by Id (optionally including soft-deleted), add, and uniqueness checks.
/// </summary>
public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default);
    Task AddAsync(Department entity, CancellationToken ct = default);

    /// <summary>Is there any active department with the given business code (excluding self)?</summary>
    Task<bool> ExistsActiveByCodeAsync(string code, Guid? excludingId = null, CancellationToken ct = default);
}
