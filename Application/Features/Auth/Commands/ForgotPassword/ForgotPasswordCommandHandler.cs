using DataAnalysis.Application.Common;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Common.Settings;
using DataAnalysis.Application.Features.Auth.Abstractions;
using DataAnalysis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataAnalysis.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IAuthRepository _authRepository; 
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ITokenService _tokenService;
    private readonly AppSettings _appSettings;
    private readonly EnvironmentSettings _environmentSettings;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IAuthRepository authRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ITokenService tokenService,
        IOptions<AppSettings> appSettings,
        IOptions<EnvironmentSettings> environmentSettings,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _authRepository = authRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _tokenService = tokenService;
        _appSettings = appSettings.Value;
        _environmentSettings = environmentSettings.Value;
        _logger = logger;
    }

    public async Task<Result> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _authRepository.FindByEmailAsync(request.Email, cancellationToken);
        
        if (user == null)
        {
            _logger.LogWarning("Password reset requested for unknown or inactive email: {Email}", request.Email);
            return Result.Ok();
        }

        var randomBytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var resetToken = Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        user.PasswordResetToken = _tokenService.HashToken(resetToken);
        user.PasswordResetExpiry = DateTime.UtcNow.AddHours(1);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Password reset token generated and saved for UserId: {UserId}", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save reset token for UserId: {UserId}", user.Id);
            throw;
        }

        try
        {
            var baseUrl = _environmentSettings.Prod
                ? _appSettings.DefaultUrlProd
                : _appSettings.DefaultUrl;

            var resetLink = $"{baseUrl}/reset-password?token={resetToken}";
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send reset email for UserId: {UserId}", user.Id);
        }

        return Result.Ok();
    }
}