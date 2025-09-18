using FileNet.Application.Common.Abstractions.Commands;

namespace FileNet.Application.Common.Commands.Create;

public abstract record CreateCommandBase<TCreateDto>(TCreateDto Model) : ICreateCommand<TCreateDto>;
