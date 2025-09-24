using FileNet.Application.Abstractions.Persistence.Repositories;
using FileNet.Application.Abstractions.Persistence;
using MediatR;

namespace FileNet.Application.Features.Departments.Commands.RenameDepartment;

public sealed class RenameDepartmentHandler(
    IDepartmentRepository departments, IUnitOfWork uow
) : IRequestHandler<RenameDepartmentCommand>
{
    public async Task Handle(RenameDepartmentCommand req, CancellationToken ct)
    {
        var dep = await departments.GetByIdAsync(req.Id, ct: ct)
                  ?? throw new KeyNotFoundException("Department not found.");

        dep.Rename(req.Name, req.Description, req.ActorId);
        await uow.SaveChangesAsync(ct);
    }
}
