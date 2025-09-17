using FileNet.Domain.Abstractions;

namespace FileNet.Domain.Base;

public abstract class EntityBase : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}
