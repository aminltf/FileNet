using FileNet.Domain.Aggregates;
using FileNet.Domain.Constants;
using FileNet.Domain.Events.Documents;

namespace FileNet.Domain.Entities;

/// <summary>
/// Document is an aggregate that belongs to an Employee by reference.
/// We keep file bytes in the aggregate (for DB storage scenario). If you later move to file store,
/// keep the same aggregate but replace Data with a FileUri value object.
/// </summary>
public sealed class Document : AggregateRoot<Guid, Guid>
{
    public Guid EmployeeId { get; private set; }

    public string Title { get; private set; } = default!;
    public string FileName { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public long SizeBytes { get; private set; }
    public DocumentCategory Category { get; private set; }

    public byte[] Data { get; private set; } = default!;

    private Document() { } // EF

    public static Document Upload(
        Guid employeeId,
        string title,
        string fileName,
        string contentType,
        long sizeBytes,
        DocumentCategory category,
        byte[] data,
        Guid? actorId)
    {
        if (employeeId == Guid.Empty) throw new ArgumentException("EmployeeId is required.", nameof(employeeId));
        if (sizeBytes <= 0) throw new ArgumentOutOfRangeException(nameof(sizeBytes), "Size must be positive.");
        if (data is null || data.Length == 0) throw new ArgumentException("Empty file data.", nameof(data));

        var doc = new Document
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            Title = NormalizeRequired(title, DomainModelConstraints.DocumentTitleMaxLen),
            FileName = NormalizeRequired(fileName, DomainModelConstraints.FileNameMaxLen),
            ContentType = NormalizeRequired(contentType, DomainModelConstraints.ContentTypeMaxLen),
            SizeBytes = sizeBytes,
            Category = category,
            Data = data
        };

        doc.MarkCreated(actorId ?? Guid.Empty);
        doc.AddDomainEvent(new DocumentUploaded(
            doc.Id,
            doc.EmployeeId,
            (short)doc.Category,
            doc.FileName,
            doc.ContentType,
            doc.SizeBytes,
            actorId ?? Guid.Empty));

        doc.CheckInvariants();
        return doc;
    }

    public void Retitle(string newTitle, Guid? actorId)
    {
        var old = Title;
        Title = NormalizeRequired(newTitle, DomainModelConstraints.DocumentTitleMaxLen);
        MarkModified(actorId ?? Guid.Empty);

        AddDomainEvent(new DocumentRetitled(Id, old, Title, actorId ?? Guid.Empty));
        CheckInvariants();
    }

    public void ReCategorize(DocumentCategory newCategory, Guid? actorId)
    {
        var oldCode = (short)Category;
        var newCode = (short)newCategory;
        if (oldCode == newCode) return;

        Category = newCategory;
        MarkModified(actorId ?? Guid.Empty);

        AddDomainEvent(new DocumentRecategorized(Id, oldCode, newCode, actorId ?? Guid.Empty));
        CheckInvariants();
    }

    /// <summary>Replace file payload + metadata atomically.</summary>
    public void ReplaceFile(string newFileName, string newContentType, long newSizeBytes, byte[] newData, Guid? actorId)
    {
        if (newSizeBytes <= 0) throw new ArgumentOutOfRangeException(nameof(newSizeBytes));
        if (newData is null || newData.Length == 0) throw new ArgumentException("Empty file data.", nameof(newData));

        var oldName = FileName;
        var oldType = ContentType;
        var oldSize = SizeBytes;

        FileName = NormalizeRequired(newFileName, DomainModelConstraints.FileNameMaxLen);
        ContentType = NormalizeRequired(newContentType, DomainModelConstraints.ContentTypeMaxLen);
        SizeBytes = newSizeBytes;
        Data = newData;

        MarkModified(actorId ?? Guid.Empty);

        AddDomainEvent(new DocumentFileReplaced(
            Id, oldName, FileName, oldType, ContentType, oldSize, SizeBytes, actorId ?? Guid.Empty));

        CheckInvariants();
    }

    public void SoftDeleteMe(string? reason, Guid? actorId)
    {
        SoftDelete(actorId ?? Guid.Empty, reason);
        AddDomainEvent(new DocumentSoftDeleted(Id, reason, actorId ?? Guid.Empty));
        CheckInvariants();
    }

    public void RestoreMe(Guid? actorId)
    {
        Restore();
        AddDomainEvent(new DocumentRestored(Id, actorId ?? Guid.Empty));
        CheckInvariants();
    }

    private static string NormalizeRequired(string value, int maxLen)
    {
        var v = (value ?? string.Empty).Trim();
        if (v.Length == 0) throw new ArgumentException("Value is required.");
        if (v.Length > maxLen) v = v[..maxLen];
        return v;
    }
}

/// <summary
/// >Document classification. Underlying type is short (maps to SMALLINT).
/// </summary>
public enum DocumentCategory : short
{
    Identity = 1,
    Education = 2,
    Contract = 3,
    Insurance = 4,
    Performance = 5,
    Other = 99
}
