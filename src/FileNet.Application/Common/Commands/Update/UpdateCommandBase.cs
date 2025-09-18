using FileNet.Application.Common.Abstractions.Commands;
using FileNet.Domain.Abstractions;

namespace FileNet.Application.Common.Commands.Update;

public abstract record UpdateCommandBase<TUpdateDto>(TUpdateDto Model) : IUpdateCommand<TUpdateDto>
    where TUpdateDto : IEntity;
