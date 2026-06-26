using DataAnalysis.Application.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DataAnalysis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected int? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var id) ? id : null;
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.Success)
            return Ok(result);

        if (result.Code == null)
            return BadRequest(result);

        return result.Code.StartsWith("auth.") 
            ? Unauthorized(result) 
            : BadRequest(result);
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.Success)
            return Ok(result);

        if (result.Code == null)
            return BadRequest(result);

        return result.Code.StartsWith("auth.")
            ? Unauthorized(result)
            : BadRequest(result);
    }
}