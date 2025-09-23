using System.ComponentModel.DataAnnotations;
using FileNet.WebFramework.Contracts.Departments;
using FileNet.WebFramework.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileNet.WebApplication.Pages.Departments;

public class CreateModel(IDepartmentService service) : PageModel
{
    private readonly IDepartmentService _service = service;

    [BindProperty] public CreateDepartmentInput Input { get; set; } = new();

    [TempData] public string? Error { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            var id = await _service.CreateAsync(new DepartmentCreateDto
            {
                Code = Input.Code,
                Name = Input.Name,
                Description = Input.Description,
            }, ct);

            return RedirectToPage("Details", new { id });
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            return Page();
        }
    }

    public class CreateDepartmentInput
    {
        [Required]
        [Display(Name = "کد")]
        public string Code { get; set; } = default!;

        [Required]
        [Display(Name = "نام")]
        public string Name { get; set; } = default!;

        [StringLength(100)]
        [Display(Name = "توضیحات")]
        public string? Description { get; set; }
    }
}
