using AutoMapper;
using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Abstractions.UoW;
using FileNet.Domain.Entities;
using MediatR;

namespace FileNet.Application.Features.Departments.Commands.Create;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateDepartmentCommandHandler(IDepartmentRepository repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken ct)
    {
        var dep = _mapper.Map<Department>(request.Model);
        await _repo.CreateAsync(dep, ct);
        await _uow.CommitAsync(ct);
        return dep.Id;
    }
}
