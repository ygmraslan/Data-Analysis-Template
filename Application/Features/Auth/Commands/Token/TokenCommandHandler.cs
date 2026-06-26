using DataAnalysis.Application.Common;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Common.Settings;
using DataAnalysis.Application.Features.Auth.Abstractions;
using DataAnalysis.Domain.Entities.Identity;
using DataAnalysis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataAnalysis.Application.Features.Auth.Commands.Token;

public class TokenCommandHandler : IRequestHandler<TokenCommand, Result<TokenCommandResponse>>
{
    private readonly IAuthRepository _authRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<TokenCommandHandler> _logger;

    public TokenCommandHandler(
        IAuthRepository authRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings,
        ILogger<TokenCommandHandler> logger)
    {
        _authRepository = authRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public async Task<Result<TokenCommandResponse>> Handle(
        TokenCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash = _tokenService.HashToken(request.Token);

        var refreshToken = await _authRepository.FindValidRefreshTokenAsync(tokenHash, cancellationToken);

        if (refreshToken == null)
        {
            _logger.LogWarning("Token renewal failed: Token not found or invalid.");
            return Result<TokenCommandResponse>.Fail("Invalid refresh token.", ErrorCodes.Auth.InvalidToken);
        }

        var user = await _authRepository.FindByIdAsync(refreshToken.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Token renewal failed: User not found or inactive. UserId: {UserId}", refreshToken.UserId);
            return Result<TokenCommandResponse>.Fail("User not found or inactive.", ErrorCodes.Auth.UserInactive);
        }

        var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var newRefreshTokenHash = _tokenService.HashToken(newRefreshToken);

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            await _authRepository.RevokeRefreshTokenAsync(refreshToken, cancellationToken);

            await _authRepository.AddRefreshTokenAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshTokenHash,
                ExpiresDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireDays),
                IpAddress = request.IpAddress
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed during token renewal for UserId: {UserId}", user.Id);
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        _logger.LogInformation("Token successfully renewed for UserId: {UserId}, Ip: {Ip}", user.Id, request.IpAddress);

        return Result<TokenCommandResponse>.Ok(new TokenCommandResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
}