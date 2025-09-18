using FileNet.Domain.Abstractions;
using MediatR;

namespace FileNet.Application.Common.Abstractions.Commands;

public interface IUpdateCommand<TUpdateDto> : IRequest<bool>
    where TUpdateDto : IEntity
{
    TUpdateDto Model { get; }
}
