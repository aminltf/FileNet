using FileNet.Application.Abstractions.Persistence.Repositories;
using FileNet.Application.Abstractions.Persistence;
using MediatR;

namespace FileNet.Application.Features.Departments.Commands.SoftDeleteDepartment;

public sealed class SoftDeleteDepartmentHandler(
    IDepartmentRepository departments, IUnitOfWork uow
) : IRequestHandler<SoftDeleteDepartmentCommand>
{
    public async Task Handle(SoftDeleteDepartmentCommand req, CancellationToken ct)
    {
        var dep = await departments.GetByIdAsync(req.Id, ct: ct)
                  ?? throw new KeyNotFoundException("Department not found.");

        dep.SoftDeleteMe(req.Reason, req.ActorId);
        await uow.SaveChangesAsync(ct);
    }
}
