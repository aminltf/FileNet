using MediatR;

namespace FileNet.Application.Common.Abstractions.Commands;

public interface IDeleteCommand : IRequest<Unit>
{
    Guid Id { get; }
}
