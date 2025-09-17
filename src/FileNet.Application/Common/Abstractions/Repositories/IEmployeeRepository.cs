using FileNet.Application.Common.Queryable;
using FileNet.Domain.Entities;

namespace FileNet.Application.Common.Abstractions.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<PageResponse<Employee>> GetPagedAsync(
        PageRequest page,
        SearchRequest search,
        SortOptions sort,
        CancellationToken ct);
}
