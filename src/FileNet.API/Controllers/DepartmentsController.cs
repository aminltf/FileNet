using FileNet.Application.Features.Departments.Commands.ChangeDepartmentCode;
using FileNet.Application.Features.Departments.Commands.CreateDepartment;
using FileNet.Application.Features.Departments.Commands.RenameDepartment;
using FileNet.Application.Features.Departments.Commands.RestoreDepartment;
using FileNet.Application.Features.Departments.Commands.SoftDeleteDepartment;
using FileNet.Application.Features.Departments.Dtos;
using FileNet.Application.Features.Departments.Queries.GetDepartmentById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FileNet.API.Controllers;

[ApiController]
[Route("api/departments")]
public sealed class DepartmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DepartmentsController(IMediator mediator) => _mediator = mediator;

    // Request DTOs
    public sealed record CreateDepartmentRequest(string Code, string Name, string? Description);
    public sealed record ChangeCodeRequest(string NewCode);
    public sealed record RenameDepartmentRequest(string Name, string? Description);
    public sealed record SoftDeleteRequest(string? Reason);

    // GET /api/departments/{id}
    [HttpGet("{id:guid}", Name = nameof(GetById))]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DepartmentDto>> GetById(Guid id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetDepartmentByIdQuery(id), ct);
        return Ok(dto);
    }

    // GET /api/departments?page=1&pageSize=20&search=foo
    //[HttpGet]
    //[ProducesResponseType(typeof(PagedResult<DepartmentDto>), StatusCodes.Status200OK)]
    //public async Task<ActionResult<PagedResult<DepartmentDto>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, CancellationToken ct = default)
    //{
    //    var result = await _mediator.Send(new ListDepartmentsQuery(page, pageSize, search), ct);
    //    return Ok(result);
    //}

    // POST /api/departments
    [HttpPost]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DepartmentDto>> Create([FromBody] CreateDepartmentRequest body, CancellationToken ct)
    {
        var actorId = GetActorIdOrNull(HttpContext.User);
        var id = await _mediator.Send(new CreateDepartmentCommand(body.Code, body.Name, body.Description, actorId), ct);

        var dto = await _mediator.Send(new GetDepartmentByIdQuery(id), ct);
        return CreatedAtAction(nameof(GetById), new { id }, dto);
    }

    // PUT /api/departments/{id}/code
    [HttpPut("{id:guid}/code")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeCode(Guid id, [FromBody] ChangeCodeRequest body, CancellationToken ct)
    {
        await _mediator.Send(new ChangeDepartmentCodeCommand(id, body.NewCode, GetActorIdOrNull(HttpContext.User)), ct);
        return NoContent();
    }

    // PUT /api/departments/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rename(Guid id, [FromBody] RenameDepartmentRequest body, CancellationToken ct)
    {
        await _mediator.Send(new RenameDepartmentCommand(id, body.Name, body.Description, GetActorIdOrNull(HttpContext.User)), ct);
        return NoContent();
    }

    // DELETE /api/departments/{id}   (Soft delete)
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SoftDelete(Guid id, [FromBody] SoftDeleteRequest? body, CancellationToken ct)
    {
        await _mediator.Send(new SoftDeleteDepartmentCommand(id, body?.Reason, GetActorIdOrNull(HttpContext.User)), ct);
        return NoContent();
    }

    // POST /api/departments/{id}/restore
    [HttpPost("{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new RestoreDepartmentCommand(id, GetActorIdOrNull(HttpContext.User)), ct);
        return NoContent();
    }

    // Helpers
    private static Guid? GetActorIdOrNull(ClaimsPrincipal user)
    {
        var raw =
            user.FindFirstValue(ClaimTypes.NameIdentifier) ??
            user.FindFirstValue("sub");

        return Guid.TryParse(raw, out var g) ? g : null;
    }
}
