using FileNet.Domain.Common.Abstractions;

namespace FileNet.Domain.Common.Base;

public abstract class AuditableBase : EntityBase, IAuditable
{
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
}
