namespace FileNet.Domain.Abstractions;

public interface IAuditable
{
    string? CreatedBy { get; set; }
    DateTimeOffset CreatedOn { get; set; }
    string? UpdatedBy { get; set; }
    DateTimeOffset? UpdatedOn { get; set; }
}
