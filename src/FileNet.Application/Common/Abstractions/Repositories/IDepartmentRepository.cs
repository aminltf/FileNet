using FileNet.Application.Common.Queryable;
using FileNet.Domain.Entities;

namespace FileNet.Application.Common.Abstractions.Repositories;

public interface IDepartmentRepository: IRepository<Department>
{
    Task<IReadOnlyList<Department>> GetLookupAsync(CancellationToken ct);
    Task<IReadOnlyList<Employee>> GetEmployeesAsync(Guid departmentId, CancellationToken ct);
    Task<PageResponse<Department>> GetPagedAsync(
        PageRequest page,
        SearchRequest search,
        SortOptions sort,
        CancellationToken ct);
}
