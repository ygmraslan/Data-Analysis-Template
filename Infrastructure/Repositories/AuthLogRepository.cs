using DataAnalysis.Application.Features.AuthLogs.Abstractions;
using DataAnalysis.Domain.Entities.Logging;
using DataAnalysis.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAnalysis.Infrastructure.Repositories;

public class AuthLogRepository : IAuthLogRepository
{
    private readonly AppDbContext _context;

    public AuthLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuthLog authLog, CancellationToken cancellationToken = default)
    {
        await _context.AuthLogs.AddAsync(authLog, cancellationToken);
    }

    public IQueryable<AuthLog> GetLogsQuery()
    {
        return _context.AuthLogs
            .Include(x => x.User)
            .AsQueryable();
    }
}