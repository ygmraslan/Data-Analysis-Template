using DataAnalysis.Application.Features.Auth.Commands.ChangePassword;
using DataAnalysis.Application.Features.Auth.Commands.ForgotPassword;
using DataAnalysis.Application.Features.Auth.Commands.Login;
using DataAnalysis.Application.Features.Auth.Commands.Logout;
using DataAnalysis.Application.Features.Auth.Commands.ResetPassword;
using DataAnalysis.Application.Features.Auth.Commands.SetupMfa;
using DataAnalysis.Application.Features.Auth.Commands.Token;
using DataAnalysis.Application.Features.Auth.Commands.VerifyMfa;
using DataAnalysis.Application.Features.Auth.Queries.GetMe;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DataAnalysis.API.Controllers;

[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [EnableRateLimiting("AuthLimit")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        command.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        command.UserAgent = Request.Headers["User-Agent"].ToString();
        var result = await _mediator.Send(command);

        if (result.Success && result.Value?.AccessToken != null)
        {
            SetAuthCookies(result.Value.AccessToken, result.Value.RefreshToken!);
            result.Value.AccessToken = null;
            result.Value.RefreshToken = null;
        }

        return HandleResult(result);
    }

    [HttpPost("setup-mfa")]
    public async Task<IActionResult> SetupMfa([FromBody] SetupMfaCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("verify-mfa")]
    [EnableRateLimiting("AuthLimit")]
    public async Task<IActionResult> VerifyMfa([FromBody] VerifyMfaCommand command)
    {
        command.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        var result = await _mediator.Send(command);

        if (result.Success && result.Value?.AccessToken != null)
        {
            SetAuthCookies(result.Value.AccessToken, result.Value.RefreshToken ?? string.Empty);
            result.Value.AccessToken = null!;
            result.Value.RefreshToken = null!;
        }

        return HandleResult(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        var command = new TokenCommand
        {
            Token = refreshToken,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty
        };

        var result = await _mediator.Send(command);

      if (result.Success && result.Value?.AccessToken != null)
        {
            SetAuthCookies(result.Value.AccessToken, result.Value.RefreshToken ?? string.Empty);
            result.Value.AccessToken = null!;
            result.Value.RefreshToken = null!;
        }

        return HandleResult(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        var command = new LogoutCommand
        {
            RefreshToken = refreshToken ?? string.Empty,
            IpAddress    = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
            UserAgent    = Request.Headers["User-Agent"].ToString()
        };
        var result = await _mediator.Send(command);

        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");

        return HandleResult(result);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        command.AuthenticatedUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        command.PasswordChangeToken = null;
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("change-password-with-token")]
    public async Task<IActionResult> ChangePasswordWithToken([FromBody] ChangePasswordCommand command)
    {
        if (string.IsNullOrEmpty(command.PasswordChangeToken))
            return BadRequest(new { message = "Token gereklidir." });

        command.AuthenticatedUserId = null;
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("ForgotPasswordLimit")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("AuthLimit")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _mediator.Send(new GetMeQuery { UserId = userId }, cancellationToken);
        return HandleResult(result);
    }

    private void SetAuthCookies(string accessToken, string refreshToken)
    {
        var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var isBehindProxy = configuration.GetValue<bool>("ReverseProxy:Enabled");

        var isProduction = !HttpContext.RequestServices
            .GetRequiredService<IWebHostEnvironment>()
            .IsDevelopment();

        // Reverse proxy (nginx) arkasinda HTTP kullaniliyorsa
        // Secure flag false olmali, yoksa tarayici cookie'yi reddeder
        var useSecureCookies = isProduction && !isBehindProxy;

        Response.Cookies.Append("accessToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = useSecureCookies,
            SameSite = useSecureCookies ? SameSiteMode.Strict : SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
        });

        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = useSecureCookies,
            SameSite = useSecureCookies ? SameSiteMode.Strict : SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(1)
        });
    }
}
