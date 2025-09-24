using FileNet.Domain.Aggregates;
using FileNet.Domain.Constants;
using FileNet.Domain.Events.Departments;

namespace FileNet.Domain.Entities;

/// <summary>
/// Department is an aggregate root. It owns only its own invariants,
/// employees are separate aggregates referencing Department by Id.
/// </summary>
public sealed class Department : AggregateRoot<Guid, Guid>
{
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    private Department() { } // EF

    public static Department Create(string code, string name, string? description, Guid? actorId)
    {
        code = NormalizeRequired(code, DomainModelConstraints.DepartmentCodeMaxLen);
        name = NormalizeRequired(name, DomainModelConstraints.DepartmentNameMaxLen);
        description = NormalizeOptional(description, DomainModelConstraints.DepartmentDescriptionMaxLen);

        var dep = new Department
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = name,
            Description = description
        };

        dep.MarkCreated(actorId ?? Guid.Empty);
        dep.AddDomainEvent(new DepartmentCreated(dep.Id, dep.Code, dep.Name, actorId));
        dep.CheckInvariants();
        return dep;
    }

    public void Rename(string newName, string? newDescription, Guid? actorId)
    {
        var oldName = Name;
        var oldDesc = Description;

        Name = NormalizeRequired(newName, DomainModelConstraints.DepartmentNameMaxLen);
        Description = NormalizeOptional(newDescription, DomainModelConstraints.DepartmentDescriptionMaxLen);
        MarkModified(actorId ?? Guid.Empty);

        AddDomainEvent(new DepartmentRenamed(Id, oldName, Name, oldDesc, Description, actorId));
        CheckInvariants();
    }

    public void ChangeCode(string newCode, Guid? actorId)
    {
        var old = Code;
        Code = NormalizeRequired(newCode, DomainModelConstraints.DepartmentCodeMaxLen);
        MarkModified(actorId ?? Guid.Empty);

        AddDomainEvent(new DepartmentCodeChanged(Id, old, Code, actorId));
        CheckInvariants();
    }

    public void SoftDeleteMe(string? reason, Guid? actorId)
    {
        SoftDelete(actorId ?? Guid.Empty, reason);
        AddDomainEvent(new DepartmentSoftDeleted(Id, reason, actorId));
        CheckInvariants();
    }

    public void RestoreMe(Guid? actorId)
    {
        Restore();
        AddDomainEvent(new DepartmentRestored(Id, actorId));
        CheckInvariants();
    }

    // local guards
    private static string NormalizeRequired(string value, int maxLen)
    {
        var v = (value ?? string.Empty).Trim();
        if (v.Length == 0) throw new ArgumentException("Value is required.");
        if (v.Length > maxLen) v = v[..maxLen];
        return v;
    }
    private static string? NormalizeOptional(string? value, int maxLen)
    {
        var v = value?.Trim();
        if (string.IsNullOrWhiteSpace(v)) return null;
        if (v!.Length > maxLen) v = v[..maxLen];
        return v;
    }
}
