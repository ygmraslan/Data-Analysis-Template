using DataAnalysis.Application.Features.ExecSummary.Abstractions;
using DataAnalysis.Application.Features.ExecSummary.Dtos;
using DataAnalysis.Domain.Entities.ExecSummary;
using DataAnalysis.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAnalysis.Infrastructure.Repositories;

public class ExecAiCacheRepository : IExecAiCacheRepository
{
    private readonly AppDbContext _context;

    public ExecAiCacheRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ExecAiCacheDto?> GetAsync(
        DateTime weekStart, 
        DateTime weekEnd, 
        string productType,
        string modelType,
        CancellationToken cancellationToken = default)
    {
        var utcStart = DateTime.SpecifyKind(weekStart.Date, DateTimeKind.Utc);
        var utcEnd = DateTime.SpecifyKind(weekEnd.Date, DateTimeKind.Utc);
        
        var entity = await _context.ExecAiCaches
            .AsNoTracking()
            .FirstOrDefaultAsync(x => 
                x.WeekStart == utcStart && 
                x.WeekEnd == utcEnd && 
                x.ProductType == productType &&
                x.ModelType == modelType, 
                cancellationToken);

        if (entity == null)
            return null;

        return MapToDto(entity);
    }

    public async Task<ExecAiCacheDto> SaveAsync(
        DateTime weekStart,
        DateTime weekEnd,
        string productType,
        string modelType,
        string summaryJson,
        int? userId,
        CancellationToken cancellationToken = default)
    {
        var utcStart = DateTime.SpecifyKind(weekStart.Date, DateTimeKind.Utc);
        var utcEnd = DateTime.SpecifyKind(weekEnd.Date, DateTimeKind.Utc);

        var existing = await _context.ExecAiCaches
            .FirstOrDefaultAsync(x => 
                x.WeekStart == utcStart && 
                x.WeekEnd == utcEnd && 
                x.ProductType == productType &&
                x.ModelType == modelType, 
                cancellationToken);

        if (existing != null)
        {
            existing.SummaryJson = summaryJson;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedByUserId = userId;
            await _context.SaveChangesAsync(cancellationToken);
            return MapToDto(existing);
        }

        var entity = new ExecAiCache
        {
            WeekStart = utcStart,
            WeekEnd = utcEnd,
            ProductType = productType,
            ModelType = modelType,
            SummaryJson = summaryJson,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        await _context.ExecAiCaches.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return MapToDto(entity);
    }

    public async Task<ExecAiCacheDto> UpdateAsync(
        int id,
        string summaryJson,
        int? userId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.ExecAiCaches
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
            throw new InvalidOperationException($"ExecAiCache with id {id} not found");

        entity.SummaryJson = summaryJson;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedByUserId = userId;

        await _context.SaveChangesAsync(cancellationToken);
        
        return MapToDto(entity);
    }

    private static ExecAiCacheDto MapToDto(ExecAiCache entity)
    {
        return new ExecAiCacheDto
        {
            Id = entity.Id,
            WeekStart = entity.WeekStart,
            WeekEnd = entity.WeekEnd,
            ProductType = entity.ProductType,
            ModelType = entity.ModelType,
            SummaryJson = entity.SummaryJson,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}