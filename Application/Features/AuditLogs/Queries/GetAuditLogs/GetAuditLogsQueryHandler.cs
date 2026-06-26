using DataAnalysis.Application.Common;
using DataAnalysis.Application.Features.AuditLogs.Abstractions;
using DataAnalysis.Application.Features.AuditLogs.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, Result<GetAuditLogsQueryResponse>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<GetAuditLogsQueryHandler> _logger;

    public GetAuditLogsQueryHandler(
        IAuditLogRepository auditLogRepository,
        ILogger<GetAuditLogsQueryHandler> logger)
    {
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task<Result<GetAuditLogsQueryResponse>> Handle(
        GetAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching audit logs. Page: {Page}, Size: {Size}", request.Page, request.PageSize);

        var query = _auditLogRepository.GetLogsQuery();

        if (request.UserId.HasValue)
            query = query.Where(x => x.UserId == request.UserId.Value);

        if (!string.IsNullOrWhiteSpace(request.Module))
            query = query.Where(x => x.Module == request.Module);

        if (!string.IsNullOrWhiteSpace(request.Action))
            query = query.Where(x => x.Action == request.Action);

        if (request.StatusCode.HasValue)
            query = query.Where(x => x.StatusCode == request.StatusCode.Value);

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
            .Select(x => new AuditLogDto
            {
                Id = x.Id,
                UserId = x.UserId,
                FullName = x.User.FirstName + " " + x.User.LastName,
                Email = x.User.Email,
                Module = x.Module,
                Action = x.Action,
                ActionDescription = x.ActionDescription,
                IpAddress = x.IpAddress,
                Browser = x.Browser,
                Method = x.Method,
                Request = x.Request,
                StatusCode = x.StatusCode,
                CreatedDate = x.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return Result<GetAuditLogsQueryResponse>.Ok(new GetAuditLogsQueryResponse
        {
            Logs = logs,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize
        });
    }
}