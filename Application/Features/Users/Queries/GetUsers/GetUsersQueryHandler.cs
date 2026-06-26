using DataAnalysis.Application.Common;
using DataAnalysis.Application.Features.Users.Abstractions;
using DataAnalysis.Application.Features.Users.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<GetUsersQueryResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUsersQueryHandler> _logger;

    public GetUsersQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUsersQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<GetUsersQueryResponse>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching users list. Page: {Page}, Size: {Size}", request.Page, request.PageSize);

        var query = _userRepository.GetUsersQuery();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(x =>
                x.FirstName.ToLower().Contains(search) ||
                x.LastName.ToLower().Contains(search) ||
                x.Email.ToLower().Contains(search));
        }

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var pagedUsers = await query
            .OrderBy(x => x.FirstName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var userIds = pagedUsers.Select(x => x.Id).ToList();
        var activeMfaUserIds = await _userRepository.GetActiveMfaUserIdsAsync(userIds, cancellationToken);

        var userDtos = pagedUsers.Select(user => new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IsActive = user.IsActive,
            HasMfa = activeMfaUserIds.Contains(user.Id),
            IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow,
            LastLoginDate = user.LastLoginDate,
            CreatedDate = user.CreatedDate
        }).ToList();

        return Result<GetUsersQueryResponse>.Ok(new GetUsersQueryResponse
        {
            Users = userDtos,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize
        });
    }
}