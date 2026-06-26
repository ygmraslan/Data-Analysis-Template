using DataAnalysis.Domain.Entities.Logging;

namespace DataAnalysis.Application.Features.AuthLogs.Abstractions;

public interface IAuthLogRepository
{
    Task AddAsync(AuthLog authLog, CancellationToken cancellationToken = default);
    IQueryable<AuthLog> GetLogsQuery();
}