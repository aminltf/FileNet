namespace FileNet.Domain.Common.Abstractions;

public interface IAuditable
{
    DateTimeOffset CreatedOn { get; set; }
    DateTimeOffset UpdatedOn { get; set; }
}
