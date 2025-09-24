using FileNet.Application.Abstractions.Persistence.Repositories;
using FileNet.Application.Abstractions.Persistence;
using FileNet.Domain.Entities;
using MediatR;

namespace FileNet.Application.Features.Departments.Commands.CreateDepartment;

public sealed class CreateDepartmentHandler(
    IDepartmentRepository departments,
    IUnitOfWork uow
) : IRequestHandler<CreateDepartmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateDepartmentCommand req, CancellationToken ct)
    {
        if (await departments.ExistsActiveByCodeAsync(req.Code, ct: ct))
            throw new InvalidOperationException("Department code already exists.");

        var entity = Department.Create(req.Code, req.Name, req.Description, req.ActorId);

        await departments.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity.Id;
    }
}
