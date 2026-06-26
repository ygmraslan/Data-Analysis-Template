using DataAnalysis.Application.Common;
using DataAnalysis.Application.Features.Auth.Abstractions;
using DataAnalysis.Application.Features.Users.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Auth.Queries.GetMe;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, Result<GetMeResponse>>
{
    private readonly IAuthRepository _authRepository;
    private readonly IUserRepository _userRepository;

    public GetMeQueryHandler(
        IAuthRepository authRepository,
        IUserRepository userRepository)
    {
        _authRepository = authRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<GetMeResponse>> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.FindByIdAsync(request.UserId, cancellationToken);

        if (user == null)
            return Result<GetMeResponse>.Fail("User not found.");

        var permissions = await _userRepository.GetUserPermissionsAsync(request.UserId, cancellationToken);

        return Result<GetMeResponse>.Ok(new GetMeResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Permissions = permissions
        });
    }
}