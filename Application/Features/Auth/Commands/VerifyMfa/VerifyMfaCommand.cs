using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.Auth.Commands.VerifyMfa;

public class VerifyMfaCommand : IRequest<Result<VerifyMfaCommandResponse>>
{
    public string MfaToken { get; set; } = string.Empty;
    public string MfaCode { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public bool IsSetupFlow { get; set; }
}

public class VerifyMfaCommandResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public bool IsSetupComplete { get; set; }
}