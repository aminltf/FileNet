using FileNet.Domain.Common.Abstractions;

namespace FileNet.Domain.Common.Base;

public abstract class EntityBase : IEntity
{
    public Guid Id { get; set; }
}
