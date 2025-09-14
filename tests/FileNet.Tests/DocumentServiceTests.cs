using FileNet.WebFramework.Contracts.Documents;
using FileNet.WebFramework.Contracts.Employees;
using FileNet.WebFramework.Enums;
using FileNet.WebFramework.Services.Implementations;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text;

namespace FileNet.Tests;

public class DocumentServiceTests
{
    private static IFormFile MakeFormFile(string name, string content, string contentType = "text/plain")
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var ms = new MemoryStream(bytes);
        return new FormFile(ms, 0, bytes.Length, "file", name) { Headers = new HeaderDictionary(), ContentType = contentType };
    }

    [Fact]
    public async Task Upload_And_Download_Works()
    {
        using var db = TestHelpers.CreateInMemoryDb();
        var empSvc = new EmployeeService(db);
        var empId = await empSvc.CreateAsync(new EmployeeCreateDto { NationalCode = "555", FirstName = "A", LastName = "B" }, CancellationToken.None);

        var docSvc = new DocumentService(db, NullLogger<DocumentService>.Instance);

        var file = MakeFormFile("note.txt", "hello");
        var id = await docSvc.UploadAsync(new DocumentUploadDto
        {
            EmployeeId = empId,
            Title = "Test",
            Category = DocumentCategory.Other,
            File = file
        }, CancellationToken.None);

        var dl = await docSvc.DownloadAsync(id, CancellationToken.None);
        dl.Should().NotBeNull();
        Encoding.UTF8.GetString(dl!.Data).Should().Be("hello");
        dl.ContentType.Should().Be("text/plain");
        dl.FileName.Should().Be("note.txt");
    }

    [Fact]
    public async Task Upload_Rejects_EmptyFile()
    {
        using var db = TestHelpers.CreateInMemoryDb();
        var empSvc = new EmployeeService(db);
        var empId = await empSvc.CreateAsync(new EmployeeCreateDto { NationalCode = "777", FirstName = "A", LastName = "B" }, CancellationToken.None);

        var docSvc = new DocumentService(db, NullLogger<DocumentService>.Instance);
        var empty = new FormFile(new MemoryStream(), 0, 0, "file", "empty.txt");

        var act = () => docSvc.UploadAsync(new DocumentUploadDto { EmployeeId = empId, Category = DocumentCategory.Other, File = empty }, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*Empty file*");
    }

    [Fact]
    public async Task Upload_Fails_When_Employee_NotFound()
    {
        using var db = TestHelpers.CreateInMemoryDb();
        var docSvc = new DocumentService(db, NullLogger<DocumentService>.Instance);

        var file = MakeFormFile("a.txt", "x");
        var act = () => docSvc.UploadAsync(new DocumentUploadDto { EmployeeId = Guid.NewGuid(), Category = DocumentCategory.Other, File = file }, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
                 .WithMessage("*Employee not found*");
    }

    [Fact]
    public async Task Delete_Removes_Document()
    {
        using var db = TestHelpers.CreateInMemoryDb();
        var empSvc = new EmployeeService(db);
        var empId = await empSvc.CreateAsync(new EmployeeCreateDto { NationalCode = "888", FirstName = "A", LastName = "B" }, CancellationToken.None);

        var docSvc = new DocumentService(db, NullLogger<DocumentService>.Instance);
        var file = MakeFormFile("a.txt", "x");
        var id = await docSvc.UploadAsync(new DocumentUploadDto { EmployeeId = empId, Category = DocumentCategory.Other, File = file }, CancellationToken.None);

        await docSvc.DeleteAsync(id, CancellationToken.None);

        var dl = await docSvc.DownloadAsync(id, CancellationToken.None);
        dl.Should().BeNull();
    }
}
