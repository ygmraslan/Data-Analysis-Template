using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<Result>
{
    public string? PasswordChangeToken { get; set; }
    public int? AuthenticatedUserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}