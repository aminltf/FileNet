namespace FileNet.Domain.Abstractions;

/// <summary>
/// Soft-delete metadata. Keep IsDeleted for cheap filtering / filtered unique indexes,
/// but always maintain the invariant with DeletedOn.
/// </summary>
public interface ISoftDeletable<TUserId>
    where TUserId : IEquatable<TUserId>
{
    bool IsDeleted { get; set; }             // mirror of DeletedOn.HasValue for perf/indexes
    TUserId? DeletedById { get; set; }
    DateTimeOffset? DeletedOn { get; set; }  // should be UTC
    string? DeleteReason { get; set; }
}
