using DataAnalysis.Application.Features.Users.Commands.CreateUser;
using DataAnalysis.Application.Features.Users.Commands.ResetUserMfa;
using DataAnalysis.Application.Features.Users.Commands.ResetUserPassword;
using DataAnalysis.Application.Features.Users.Commands.ToggleUserStatus;
using DataAnalysis.Application.Features.Users.Commands.UnlockUser;
using DataAnalysis.Application.Features.Users.Commands.UpdateUser;
using DataAnalysis.Application.Features.Users.Queries.GetUserById;
using DataAnalysis.Application.Features.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysis.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class UsersController : BaseController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "Users.View")]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
    {
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Users.ViewDetail")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery { Id = id });
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "Users.Create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Users.Update")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Users.ToggleStatus")]
    public async Task<IActionResult> ToggleUserStatus(int id)
    {
        var result = await _mediator.Send(new ToggleUserStatusCommand { Id = id });
        return HandleResult(result);
    }

    [HttpPatch("{id}/unlock")]
    [Authorize(Policy = "Users.Unlock")]
    public async Task<IActionResult> UnlockUser(int id)
    {
        var result = await _mediator.Send(new UnlockUserCommand { Id = id });
        return HandleResult(result);
    }

    [HttpPatch("{id}/reset-mfa")]
    [Authorize(Policy = "Users.ResetMfa")]
    public async Task<IActionResult> ResetUserMfa(int id)
    {
        var result = await _mediator.Send(new ResetUserMfaCommand { Id = id });
        return HandleResult(result);
    }

    [HttpPatch("{id}/reset-password")]
    [Authorize(Policy = "Users.ResetPassword")]
    public async Task<IActionResult> ResetUserPassword(int id)
    {
        var result = await _mediator.Send(new ResetUserPasswordCommand { Id = id });
        return HandleResult(result);
    }
}