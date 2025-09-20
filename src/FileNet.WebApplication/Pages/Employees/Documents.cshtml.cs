using FileNet.WebFramework.Contracts.Documents;
using FileNet.WebFramework.Contracts.Employees;
using FileNet.WebFramework.Services.Abstractions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FileNet.WebFramework.Enums;

namespace FileNet.WebApplication.Pages.Employees;

public class DocumentsModel(
    IEmployeeService employeeService,
    IDocumentService documentService) : PageModel
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IDocumentService _documentService = documentService;

    [FromRoute(Name = "id")] public Guid EmployeeId { get; set; }

    public EmployeeDto? Employee { get; private set; }
    public IReadOnlyList<DocumentDto> Docs { get; private set; } = [];

    [BindProperty] public UploadInput Input { get; set; } = new();

    [TempData] public string? Success { get; set; }
    [TempData] public string? Error { get; set; }

    public class UploadInput
    {
        [StringLength(256)]
        public string? Title { get; set; }

        [Required]
        public DocumentCategory Category { get; set; } = DocumentCategory.Other;

        [Required]
        public IFormFile File { get; set; } = default!;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        Employee = await _employeeService.GetByIdAsync(EmployeeId, ct);
        if (Employee is null) return NotFound();

        Docs = await _documentService.GetByEmployeeAsync(EmployeeId, ct);
        return Page();
    }

    public async Task<IActionResult> OnPostUploadAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return await ReloadAndReturnAsync(ct);

        try
        {
            await _documentService.UploadAsync(new DocumentUploadDto
            {
                EmployeeId = EmployeeId,
                Title = Input.Title,
                Category = Input.Category,
                File = Input.File
            }, ct);

            Success = "Document uploaded successfully.";
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        return RedirectToPage(new { id = EmployeeId });
    }

    public async Task<IActionResult> OnGetDownloadAsync(Guid docId, CancellationToken ct)
    {
        var dl = await _documentService.DownloadAsync(docId, ct);
        if (dl is null) return NotFound();
        return File(dl.Data, dl.ContentType, dl.FileName);
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid docId, CancellationToken ct)
    {
        await _documentService.DeleteAsync(docId, ct);
        Success = "Document deleted.";
        return RedirectToPage(new { id = EmployeeId });
    }

    private async Task<PageResult> ReloadAndReturnAsync(CancellationToken ct)
    {
        Employee = await _employeeService.GetByIdAsync(EmployeeId, ct);
        Docs = await _documentService.GetByEmployeeAsync(EmployeeId, ct);
        return Page();
    }

    public static IEnumerable<(byte Value, string Name)> Categories =>
        Enum.GetValues<DocumentCategory>().Select(e => ((byte)e, e.ToString()));
}
