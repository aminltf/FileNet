using MediatR;

namespace FileNet.Application.Common.Abstractions.Commands;

public interface ICreateCommand<TCreateDto> : IRequest<Guid>
{
    TCreateDto Model { get; }
}
