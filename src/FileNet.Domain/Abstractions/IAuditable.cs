namespace FileNet.Domain.Abstractions;

/// <summary>
/// Creation & modification audit info. Store all timestamps in UTC.
/// </summary>
public interface IAuditable<TUserId>
    where TUserId : IEquatable<TUserId>
{
    /// <remarks>May be null for system actions (seeders, jobs...)</remarks>
    TUserId? CreatedById { get; set; }
    DateTimeOffset CreatedOn { get; set; }   // should be UTC

    TUserId? ModifiedById { get; set; }
    DateTimeOffset? ModifiedOn { get; set; } // null when never modified
}
