using DataAnalysis.Application.Features.AuditLogs.Abstractions;
using DataAnalysis.Domain.Entities.Logging;
using DataAnalysis.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAnalysis.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog auditLog, CancellationToken ct = default)
        => await _context.Set<AuditLog>().AddAsync(auditLog, ct);

    public IQueryable<AuditLog> GetLogsQuery()
    => _context.Set<AuditLog>().AsNoTracking();    
}