using FileNet.Application.Abstractions.Persistence.Repositories;
using FileNet.Application.Abstractions.Persistence;
using MediatR;

namespace FileNet.Application.Features.Departments.Commands.RestoreDepartment;

public sealed class RestoreDepartmentHandler(
    IDepartmentRepository departments, IUnitOfWork uow
) : IRequestHandler<RestoreDepartmentCommand>
{
    public async Task Handle(RestoreDepartmentCommand req, CancellationToken ct)
    {
        var dep = await departments.GetByIdAsync(req.Id, includeDeleted: true, ct: ct)
                  ?? throw new KeyNotFoundException("Department not found.");

        dep.RestoreMe(req.ActorId);
        await uow.SaveChangesAsync(ct);
    }
}
