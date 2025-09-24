using FileNet.Application.Abstractions.Persistence.Repositories;
using FileNet.Application.Abstractions.Persistence;
using MediatR;

namespace FileNet.Application.Features.Departments.Commands.ChangeDepartmentCode;

public sealed class ChangeDepartmentCodeHandler(
    IDepartmentRepository departments,
    IUnitOfWork uow
) : IRequestHandler<ChangeDepartmentCodeCommand>
{
    public async Task Handle(ChangeDepartmentCodeCommand req, CancellationToken ct)
    {
        var dep = await departments.GetByIdAsync(req.Id, ct: ct)
                  ?? throw new KeyNotFoundException("Department not found.");

        if (await departments.ExistsActiveByCodeAsync(req.NewCode, excludingId: dep.Id, ct: ct))
            throw new InvalidOperationException("New code already in use.");

        dep.ChangeCode(req.NewCode, req.ActorId);
        await uow.SaveChangesAsync(ct);
    }
}
