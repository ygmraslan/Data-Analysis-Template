using DataAnalysis.Application.Common;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.Auth.Abstractions;
using DataAnalysis.Domain.Entities.Identity;
using DataAnalysis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.Auth.Commands.SetupMfa;

public class SetupMfaCommandHandler : IRequestHandler<SetupMfaCommand, Result<SetupMfaCommandResponse>>
{
    private readonly IAuthRepository _authRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IMfaService _mfaService;
    private readonly ILogger<SetupMfaCommandHandler> _logger;

    public SetupMfaCommandHandler(
        IAuthRepository authRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IMfaService mfaService,
        ILogger<SetupMfaCommandHandler> logger)
    {
        _authRepository = authRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _mfaService = mfaService;
        _logger = logger;
    }

   public async Task<Result<SetupMfaCommandResponse>> Handle(
    SetupMfaCommand request,
    CancellationToken cancellationToken)
    {
        var tokenHash = _tokenService.HashToken(request.MfaToken);
        var session = await _authRepository.FindActiveMfaSessionTokenAsync(tokenHash, cancellationToken);

        if (session == null)
        {
            _logger.LogWarning("MFA setup failed: Invalid or expired MFA token.");
            return Result<SetupMfaCommandResponse>.Fail("Invalid or expired MFA token.", ErrorCodes.Auth.InvalidToken);
        }

        var user = await _authRepository.FindByIdAsync(session.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("MFA setup failed: User not found. UserId: {UserId}", session.UserId);
            return Result<SetupMfaCommandResponse>.Fail("User not found or inactive.", ErrorCodes.Auth.UserInactive);
        }

        var secret = _mfaService.GenerateMfaSecret();
        var qrCodeUrl = _mfaService.GenerateQrCodeUri(user.Email, secret);

        var existingMfa = await _authRepository.FindPendingMfaByUserIdAsync(user.Id, cancellationToken);

        if (existingMfa == null)
            existingMfa = await _authRepository.FindResetMfaByUserIdAsync(user.Id, cancellationToken);

        if (existingMfa == null)
        {
            await _authRepository.AddMfaAsync(new UserMfa
            {
                UserId = user.Id,
                MfaSecret = secret,
                IsVerified = false,
                IsEnabled = true,
                CreatedDate = DateTime.UtcNow
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            await _authRepository.UpdateMfaSecretAsync(user.Id, secret, cancellationToken);
        }

        _logger.LogInformation("MFA setup initiated. UserId: {UserId}", user.Id);

        return Result<SetupMfaCommandResponse>.Ok(new SetupMfaCommandResponse
        {
            QrCodeBase64 = _mfaService.GenerateQrCodeBase64(qrCodeUrl),
            ManualEntryKey = secret
        });
    }
}