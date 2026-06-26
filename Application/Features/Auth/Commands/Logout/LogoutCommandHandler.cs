using DataAnalysis.Application.Common;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.Auth.Abstractions;
using DataAnalysis.Application.Features.AuthLogs.Abstractions;
using DataAnalysis.Domain.Entities.Logging;
using DataAnalysis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IAuthRepository _authRepository;
    private readonly IAuthLogRepository _authLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IAuthRepository authRepository,
        IAuthLogRepository authLogRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ILogger<LogoutCommandHandler> logger)
    {
        _authRepository = authRepository;
        _authLogRepository = authLogRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            return Result.Ok("Logged out successfully.");

        var tokenHash = _tokenService.HashToken(request.RefreshToken);
        var refreshToken = await _authRepository.FindValidRefreshTokenAsync(tokenHash, cancellationToken);

        if (refreshToken == null)
        {
            _logger.LogInformation("Logout: Token not found or already revoked.");
            return Result.Ok("Logged out successfully.");
        }

        try
        {
            await _authRepository.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("User {UserId} logged out successfully.", refreshToken.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during logout for UserId: {UserId}", refreshToken.UserId);
            throw;
        }

        await WriteAuthLogAsync(refreshToken.UserId, request);

        return Result.Ok("Logged out successfully.");
    }

    private async Task WriteAuthLogAsync(int userId, LogoutCommand request)
    {
        try
        {
            var log = new AuthLog
            {
                UserId    = userId,
                Email     = string.Empty,
                Success   = true,
                Reason    = "Çıkış yapıldı",
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                Browser   = BrowserParser.Parse(request.UserAgent),
            };

            await _authLogRepository.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AuthLog write failed during logout. UserId: {UserId}", userId);
        }
    }
}