using DataAnalysis.Application.Features.AuthLogs.Queries.GetAuthLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysis.API.Controllers;

[Authorize]
[Route("api/auth-logs")]
public class AuthLogsController : BaseController
{
    private readonly IMediator _mediator;

    public AuthLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "AuthLogs.View")]
    public async Task<IActionResult> GetAuthLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? email = null,
        [FromQuery] bool? success = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAuthLogsQuery
        {
            Page      = page,
            PageSize  = pageSize,
            Email     = email,
            Success   = success,
            StartDate = startDate,
            EndDate   = endDate
        }, cancellationToken);

        return HandleResult(result);
    }
}