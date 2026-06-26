using DataAnalysis.Application.Features.AuditLogs.Queries.GetAuditLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysis.API.Controllers;

[Authorize]
[Route("api/audit-logs")]
public class AuditLogsController : BaseController
{
    private readonly IMediator _mediator;

    public AuditLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "AuditLogs.View")]
    public async Task<IActionResult> GetAuditLogs([FromQuery] GetAuditLogsQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}