using AutoMapper;
using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Abstractions.UoW;
using FileNet.Domain.Entities;
using MediatR;

namespace FileNet.Application.Features.Employees.Commands.Update;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, bool>
{
    private readonly IEmployeeRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateEmployeeCommandHandler(IEmployeeRepository repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateEmployeeCommand request, CancellationToken ct)
    {
        var dep = await _repo.GetByIdAsync(request.Model.Id, ct);
        if (dep is null)
            return false;

        _mapper.Map<Employee>(request.Model);
        await _uow.CommitAsync(ct);
        return true;
    }
}
