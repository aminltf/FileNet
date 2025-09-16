using FileNet.WebFramework.Contracts.Common;
using FileNet.WebFramework.Contracts.Departments;
using FileNet.WebFramework.Contracts.Employees;
using FileNet.WebFramework.Entities;

namespace FileNet.WebFramework.Services.Abstractions;

public interface IDepartmentService
{
    Task<Guid> CreateAsync(DepartmentCreateDto dto, CancellationToken ct);
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken ct);
    Task<DepartmentDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task UpdateAsync(DepartmentUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<DepartmentLookupDto>> GetLookupAsync(CancellationToken ct);
    Task<IReadOnlyList<EmployeeDto>> GetEmployeesAsync(Guid departmentId, CancellationToken ct);
    Task<PageResponse<Department>> GetPagedAsync(
        PageRequest page,
        SearchRequest search,
        SortOptions sort,
        CancellationToken ct);
}
