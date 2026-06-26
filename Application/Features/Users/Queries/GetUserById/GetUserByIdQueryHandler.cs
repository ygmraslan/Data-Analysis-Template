using DataAnalysis.Application.Common;
using DataAnalysis.Application.Features.Users.Abstractions; 
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdQueryResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository, 
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<GetUserByIdQueryResponse>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching detailed user profile for UserId: {UserId}", request.Id);

        var user = await _userRepository.FindByIdAsNoTrackingAsync(request.Id, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User retrieval failed: UserId {UserId} not found.", request.Id);
            return Result<GetUserByIdQueryResponse>.Fail("User not found.", ErrorCodes.Users.UserNotFound);
        }

        var hasMfa = await _userRepository.HasActiveMfaAsync(user.Id, cancellationToken);

        return Result<GetUserByIdQueryResponse>.Ok(new GetUserByIdQueryResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IsActive = user.IsActive,
            HasMfa = hasMfa,
            IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow,
            LockoutEnd = user.LockoutEnd,
            LastLoginDate = user.LastLoginDate,
            CreatedDate = user.CreatedDate,
            UpdatedDate = user.UpdatedDate
        });
    }
}