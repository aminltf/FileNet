using FileNet.Domain.Entities;

namespace FileNet.Application.Abstractions.Persistence.Repositories;

/// <summary>
/// Thin repository for Employee aggregate: load, add, and code uniqueness checks.
/// </summary>
public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default);
    Task AddAsync(Employee entity, CancellationToken ct = default);

    Task<bool> ExistsActiveByNationalCodeAsync(string nationalCode, Guid? excludingId = null, CancellationToken ct = default);
    Task<bool> ExistsActiveByEmployeeCodeAsync(string employeeCode, Guid? excludingId = null, CancellationToken ct = default);
}
