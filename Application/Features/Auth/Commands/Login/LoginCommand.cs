using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<Result<LoginCommandResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}

public class LoginCommandResponse
{
    public bool RequiresMfa { get; set; }
    public bool RequiresMfaSetup { get; set; }
    public bool RequiresPasswordChange { get; set; }
    public string? MfaToken { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? PasswordChangeToken { get; set; }
}