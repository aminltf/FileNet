using FileNet.Application.Common.Abstractions.Commands;

namespace FileNet.Application.Common.Commands.Delete;

public abstract record DeleteCommandBase(Guid Id) : IDeleteCommand;
