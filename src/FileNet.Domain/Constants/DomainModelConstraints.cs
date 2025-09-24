namespace FileNet.Domain.Constants;

/// <summary>
/// Central place for max lengths used in EF configurations and validation.
/// </summary>
public static class DomainModelConstraints
{
    // Department
    public const int DepartmentCodeMaxLen = 20;
    public const int DepartmentNameMaxLen = 150;
    public const int DepartmentDescriptionMaxLen = 1000;

    // Employee
    public const int NationalCodeMaxLen = 20;
    public const int EmployeeCodeMaxLen = 20;
    public const int FirstNameMaxLen = 100;
    public const int LastNameMaxLen = 100;

    // Document
    public const int DocumentTitleMaxLen = 200;
    public const int FileNameMaxLen = 260;      // practical filesystem-friendly length
    public const int ContentTypeMaxLen = 127;   // RFC-ish (e.g., "application/pdf")
}
