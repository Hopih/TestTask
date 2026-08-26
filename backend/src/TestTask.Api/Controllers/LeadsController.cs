using Microsoft.AspNetCore.Mvc;
using TestTask.Api.Contracts;
using TestTask.Api.Domain;
using TestTask.Api.Services;

namespace TestTask.Api.Controllers;

[ApiController]
[Route("api/leads")]
public sealed class LeadsController(LeadService leads) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LeadDto>>> List(
        [FromQuery] LeadStatus? status,
        CancellationToken cancellationToken)
    {
        return Ok(await leads.ListAsync(status, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<LeadDto>> Create(
        [FromBody] CreateLeadRequest request,
        CancellationToken cancellationToken)
    {
        var created = await leads.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(List), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<LeadDto>> UpdateStatus(
        Guid id,
        [FromBody] UpdateLeadStatusRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await leads.UpdateStatusAsync(id, request.Status, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }
}
