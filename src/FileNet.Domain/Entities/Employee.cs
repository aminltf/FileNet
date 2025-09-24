using FileNet.Domain.Aggregates;
using FileNet.Domain.Constants;
using FileNet.Domain.Events.Employees;

namespace FileNet.Domain.Entities;

/// <summary>
/// Employee is its own aggregate and references Department by Id.
/// </summary>
public sealed class Employee : AggregateRoot<Guid, Guid>
{
    // FK to Department aggregate
    public Guid DepartmentId { get; private set; }

    // Identity and codes
    public string NationalCode { get; private set; } = default!;
    public string EmployeeCode { get; private set; } = default!;

    // Personal info
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public Gender Gender { get; private set; }

    private Employee() { } // EF

    public static Employee Create(
        Guid departmentId,
        string nationalCode,
        string employeeCode,
        string firstName,
        string lastName,
        Gender gender,
        Guid? actorId)
    {
        if (departmentId == Guid.Empty) throw new ArgumentException("DepartmentId is required.", nameof(departmentId));

        var emp = new Employee
        {
            Id = Guid.NewGuid(),
            DepartmentId = departmentId,
            NationalCode = NormalizeRequired(nationalCode, DomainModelConstraints.NationalCodeMaxLen),
            EmployeeCode = NormalizeRequired(employeeCode, DomainModelConstraints.EmployeeCodeMaxLen),
            FirstName = NormalizeRequired(firstName, DomainModelConstraints.FirstNameMaxLen),
            LastName = NormalizeRequired(lastName, DomainModelConstraints.LastNameMaxLen),
            Gender = gender
        };

        emp.MarkCreated(actorId ?? Guid.Empty);
        emp.AddDomainEvent(new EmployeeCreated(
            emp.Id,
            emp.DepartmentId,
            emp.NationalCode,
            emp.EmployeeCode,
            (short)emp.Gender,
            actorId ?? Guid.Empty));

        emp.CheckInvariants();
        return emp;
    }

    public void Rename(string firstName, string lastName, Guid? actorId)
    {
        var oldFirst = FirstName;
        var oldLast = LastName;

        FirstName = NormalizeRequired(firstName, DomainModelConstraints.FirstNameMaxLen);
        LastName = NormalizeRequired(lastName, DomainModelConstraints.LastNameMaxLen);
        MarkModified(actorId ?? Guid.Empty);

        AddDomainEvent(new EmployeeRenamed(Id, oldFirst, oldLast, FirstName, LastName, actorId ?? Guid.Empty));
        CheckInvariants();
    }

    public void ChangeDepartment(Guid newDepartmentId, Guid? actorId)
    {
        if (newDepartmentId == Guid.Empty) throw new ArgumentException("DepartmentId is required.", nameof(newDepartmentId));

        var old = DepartmentId;
        if (old == newDepartmentId) return;

        DepartmentId = newDepartmentId;
        MarkModified(actorId ?? Guid.Empty);

        AddDomainEvent(new EmployeeTransferred(Id, old, DepartmentId, actorId ?? Guid.Empty));
        CheckInvariants();
    }

    public void ChangeCodes(string nationalCode, string employeeCode, Guid? actorId)
    {
        var oldNat = NationalCode;
        var oldEmp = EmployeeCode;

        NationalCode = NormalizeRequired(nationalCode, DomainModelConstraints.NationalCodeMaxLen);
        EmployeeCode = NormalizeRequired(employeeCode, DomainModelConstraints.EmployeeCodeMaxLen);
        MarkModified(actorId ?? Guid.Empty);

        AddDomainEvent(new EmployeeCodesChanged(Id, oldNat, NationalCode, oldEmp, EmployeeCode, actorId ?? Guid.Empty));
        CheckInvariants();
    }

    public void ChangeGender(Gender newGender, Guid? actorId)
    {
        var oldCode = (short)Gender;
        var newCode = (short)newGender;
        if (oldCode == newCode) return;

        Gender = newGender;
        MarkModified(actorId ?? Guid.Empty);

        AddDomainEvent(new EmployeeGenderChanged(Id, oldCode, newCode, actorId ?? Guid.Empty));
        CheckInvariants();
    }

    public void SoftDeleteMe(string? reason, Guid? actorId)
    {
        SoftDelete(actorId ?? Guid.Empty, reason);
        AddDomainEvent(new EmployeeSoftDeleted(Id, reason, actorId));
        CheckInvariants();
    }

    public void RestoreMe(Guid? actorId)
    {
        Restore();
        AddDomainEvent(new EmployeeRestored(Id, actorId));
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

/// <summary>
/// Simple gender enumeration. Underlying type is short for compact storage.
/// </summary>
public enum Gender : short
{
    Unknown = 0,
    Male = 1,
    Female = 2
}
