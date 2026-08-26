using Microsoft.AspNetCore.Mvc;
using TestTask.Api.Contracts;
using TestTask.Api.Domain;
using TestTask.Api.Services;

namespace TestTask.Api.Integration;

/// <summary>
/// Single CRM resource: GET creates a snapshot of leads, POST creates a lead.
/// Same contract as the internal UI, isolated behind an API key.
/// </summary>
[ApiController]
[Route("api/integration/leads")]
[ApiKey]
public sealed class IntegrationLeadsController(LeadService leads) : ControllerBase
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
        return Created("/api/integration/leads", created);
    }
}
