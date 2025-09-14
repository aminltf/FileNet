using FileNet.WebFramework.Contracts.Employees;

namespace FileNet.WebFramework.Services.Abstractions;

public interface IEmployeeService
{
    Task<Guid> CreateAsync(EmployeeCreateDto dto, CancellationToken ct);
    Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken ct);
    Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task UpdateAsync(EmployeeUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
