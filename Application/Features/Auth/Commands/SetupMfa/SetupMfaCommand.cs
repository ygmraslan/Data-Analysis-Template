using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.Auth.Commands.SetupMfa;

public class SetupMfaCommand : IRequest<Result<SetupMfaCommandResponse>>
{
    public string MfaToken { get; set; } = string.Empty;
}

public class SetupMfaCommandResponse
{
    public string QrCodeBase64 { get; set; } = string.Empty;
    public string ManualEntryKey { get; set; } = string.Empty;
}