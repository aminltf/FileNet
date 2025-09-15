using FileNet.WebFramework.Contracts.Departments;

namespace FileNet.WebFramework.Services.Abstractions;

public interface IDepartmentService
{
    Task<Guid> CreateAsync(DepartmentCreateDto dto, CancellationToken ct);
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken ct);
    Task<DepartmentDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task UpdateAsync(DepartmentUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
