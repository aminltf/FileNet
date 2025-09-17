using AutoMapper;
using FileNet.Application.Common.Abstractions.Repositories;
using FileNet.Application.Common.Abstractions.UoW;
using FileNet.Domain.Entities;
using MediatR;

namespace FileNet.Application.Features.Employees.Commands.Create;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Guid>
{
    private readonly IEmployeeRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateEmployeeCommandHandler(IEmployeeRepository repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateEmployeeCommand request, CancellationToken ct)
    {
        var emp = _mapper.Map<Employee>(request.Model);
        await _repo.CreateAsync(emp, ct);
        await _uow.CommitAsync(ct);
        return emp.Id;
    }
}
