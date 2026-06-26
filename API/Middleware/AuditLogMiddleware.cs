using DataAnalysis.Application.Common;
using DataAnalysis.Application.Features.AuditLogs.Abstractions;
using DataAnalysis.Application.Features.Permissions.Abstractions;
using DataAnalysis.Domain.Entities.Logging;
using DataAnalysis.Domain.Interfaces;
using System.Security.Claims;

namespace DataAnalysis.API.Middleware;

public class AuditLogMiddleware
{
    private readonly RequestDelegate _next;

    public AuditLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (!context.User.Identity?.IsAuthenticated ?? true)
            return;

        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            return;

        var endpoint = context.GetEndpoint();
        var authorizeData = endpoint?.Metadata
            .GetOrderedMetadata<Microsoft.AspNetCore.Authorization.IAuthorizeData>()
            .FirstOrDefault(x => x.Policy != null);

        if (authorizeData?.Policy == null)
            return;

        var policyName = authorizeData.Policy;

        using var scope = context.RequestServices.CreateScope();
        var permissionRepository = scope.ServiceProvider.GetRequiredService<IPermissionRepository>();
        var auditLogRepository   = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();
        var unitOfWork           = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var moduleAction = await permissionRepository.FindByPolicyNameAsync(policyName);
        if (moduleAction == null)
            return;

        var userAgent = context.Request.Headers["User-Agent"].ToString();
        
        var auditLog = new AuditLog
        {
            UserId            = userId,
            Module            = moduleAction.Value.Module,
            Action            = moduleAction.Value.Action,
            ActionDescription = moduleAction.Value.ActionDescription,
            IpAddress         = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
            UserAgent         = userAgent,
            Browser           = BrowserParser.Parse(userAgent),
            Method            = context.Request.Method,
            Request           = context.Request.Path,
            StatusCode        = context.Response.StatusCode,
            CreatedDate       = DateTime.UtcNow
        };

        try
        {
            await auditLogRepository.AddAsync(auditLog);
            await unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<AuditLogMiddleware>>();
            logger.LogError(ex, "AuditLog kaydedilemedi. UserId={UserId}, Path={Path}", userId, context.Request.Path);
        }
    }
}