using DataAnalysis.Application.Common;
using DataAnalysis.Application.Features.AuthLogs.Abstractions;
using DataAnalysis.Application.Features.AuthLogs.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.AuthLogs.Queries.GetAuthLogs;

public class GetAuthLogsQueryHandler : IRequestHandler<GetAuthLogsQuery, Result<GetAuthLogsQueryResponse>>
{
    private readonly IAuthLogRepository _authLogRepository;
    private readonly ILogger<GetAuthLogsQueryHandler> _logger;

    public GetAuthLogsQueryHandler(
        IAuthLogRepository authLogRepository,
        ILogger<GetAuthLogsQueryHandler> logger)
    {
        _authLogRepository = authLogRepository;
        _logger = logger;
    }

    public async Task<Result<GetAuthLogsQueryResponse>> Handle(
        GetAuthLogsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching auth logs. Page: {Page}, Size: {Size}", request.Page, request.PageSize);

        var query = _authLogRepository.GetLogsQuery();

        if (!string.IsNullOrWhiteSpace(request.Email))
            query = query.Where(x => x.Email.Contains(request.Email));

        if (request.Success.HasValue)
            query = query.Where(x => x.Success == request.Success.Value);

        if (request.StartDate.HasValue)
            query = query.Where(x => x.CreatedDate >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(x => x.CreatedDate <= request.EndDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var logs = await query
            .OrderByDescending(x => x.CreatedDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new AuthLogDto
            {
                Id          = x.Id,
                UserId      = x.UserId,
                FullName    = x.User != null ? x.User.FirstName + " " + x.User.LastName : string.Empty,
                Email       = x.User != null ? x.User.Email : x.Email,
                Success     = x.Success,
                Reason      = x.Reason,
                IpAddress   = x.IpAddress,
                Browser     = x.Browser,
                UserAgent   = x.UserAgent,
                CreatedDate = x.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return Result<GetAuthLogsQueryResponse>.Ok(new GetAuthLogsQueryResponse
        {
            Logs        = logs,
            TotalCount  = totalCount,
            TotalPages  = totalPages,
            CurrentPage = request.Page,
            PageSize    = request.PageSize
        });
    }
}