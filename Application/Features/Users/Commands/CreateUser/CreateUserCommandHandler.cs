using DataAnalysis.Application.Common;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Common.Settings;
using DataAnalysis.Application.Features.Users.Abstractions;
using DataAnalysis.Domain.Entities.Identity;
using DataAnalysis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataAnalysis.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<int>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly IEmailService _emailService;
    private readonly AppSettings _appSettings;
    private readonly EnvironmentSettings _environmentSettings;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        IEmailService emailService,
        IOptions<AppSettings> appSettings,
        IOptions<EnvironmentSettings> environmentSettings,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _emailService = emailService;
        _appSettings = appSettings.Value;
        _environmentSettings = environmentSettings.Value;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var userExists = await _userRepository.EmailExistsAsync(request.Email, null, cancellationToken);

        if (userExists)
        {
            _logger.LogWarning("User creation failed. Email already exists: {Email}", request.Email);
            return Result<int>.Fail("A user with this email already exists.", ErrorCodes.Users.EmailAlreadyExists);
        }

        var temporaryPassword = _passwordService.GenerateTemporaryPassword();

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = _passwordService.HashPassword(temporaryPassword),
            IsActive = true,
            IsPasswordChangeRequired = true
        };

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            await _userRepository.AddMfaAsync(new UserMfa
            {
                UserId = user.Id,
                MfaSecret = string.Empty,
                IsVerified = false,
                IsEnabled = true,
                CreatedDate = DateTime.UtcNow
            }, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("User and MFA record created. UserId: {UserId}, Email: {Email}", user.Id, user.Email);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "User creation failed, transaction rolled back. Email: {Email}", request.Email);
            throw;
        }

        try
        {
            var baseUrl = _environmentSettings.Prod
                ? _appSettings.DefaultUrlProd
                : _appSettings.DefaultUrl;

            var loginLink = $"{baseUrl}/login";
            await _emailService.SendWelcomeEmailAsync(
                user.Email,
                $"{user.FirstName} {user.LastName}",
                temporaryPassword,
                loginLink);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Welcome email delivery failed for UserId: {UserId}", user.Id);
        }

        return Result<int>.Ok(user.Id, "User created successfully.");
    }
}