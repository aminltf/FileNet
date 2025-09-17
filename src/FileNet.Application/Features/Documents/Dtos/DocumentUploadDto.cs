using FileNet.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace FileNet.Application.Features.Documents.Dtos;

public class DocumentUploadDto
{
    public Guid EmployeeId { get; set; }
    public string? Title { get; set; }
    public DocumentCategory Category { get; set; }
    public IFormFile File { get; set; } = default!;
}
