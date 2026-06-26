using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.Auth.Commands.Token;

public class TokenCommand : IRequest<Result<TokenCommandResponse>>
{
    public string Token { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
}

public class TokenCommandResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}