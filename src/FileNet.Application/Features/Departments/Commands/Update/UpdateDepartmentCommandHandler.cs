using AutoMapper;
using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Abstractions.UoW;
using FileNet.Domain.Entities;
using MediatR;

namespace FileNet.Application.Features.Departments.Commands.Update;

public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, bool>
{
    private readonly IDepartmentRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateDepartmentCommandHandler(IDepartmentRepository repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateDepartmentCommand request, CancellationToken ct)
    {
        var dep = await _repo.GetByIdAsync(request.Model.Id, ct);
        if (dep is null)
            return false;

        _mapper.Map<Department>(request.Model);
        await _uow.CommitAsync(ct);
        return true;
    }
}
