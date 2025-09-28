using FileNet.WebFramework.Contracts.Common;
using FileNet.WebFramework.Contracts.Dependents;

namespace FileNet.WebFramework.Services.Abstractions;

public interface IDependentService
{
    Task<Guid> CreateAsync(DependentCreateDto dto, CancellationToken ct);
    Task<IReadOnlyList<DependentDto>> GetAllAsync(CancellationToken ct);
    Task<DependentDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task UpdateAsync(DependentUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<DependentDto>> GetByEmployeeAsync(Guid employeeId, CancellationToken ct);
    Task<PageResponse<DependentDto>> GetPagedAsync(PagedRequest request, CancellationToken ct);
}
