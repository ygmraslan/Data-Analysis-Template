using DataAnalysis.Application.Features.Permissions.Commands.AssignPermissions;
using DataAnalysis.Application.Features.Permissions.Queries.GetPermissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysis.API.Controllers;

[Authorize]
[Route("api/permissions")]
public class PermissionsController : BaseController
{
    private readonly IMediator _mediator;

    public PermissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userId}")]
    [Authorize(Policy = "Permissions.View")]
    public async Task<IActionResult> GetPermissions(int userId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPermissionsQuery { UserId = userId }, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("assign")]
    [Authorize(Policy = "Permissions.Assign")]
    public async Task<IActionResult> AssignPermissions(AssignPermissionsCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}