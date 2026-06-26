using DataAnalysis.Domain.Entities.Logging;

namespace DataAnalysis.Application.Features.AuditLogs.Abstractions;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken ct = default);
    IQueryable<AuditLog>GetLogsQuery();
}