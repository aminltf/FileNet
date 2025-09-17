using FileNet.Domain.Abstractions;

namespace FileNet.Domain.Base;

public abstract class AuditableBase : EntityBase, IAuditable
{
    public string? CreatedBy { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; } = DateTimeOffset.UtcNow;
}
