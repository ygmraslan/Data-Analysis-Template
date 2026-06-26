using DataAnalysis.Application.Features.ExecSummary.Dtos;

namespace DataAnalysis.Application.Features.ExecSummary.Abstractions;

public interface IExecAiCacheRepository
{
    Task<ExecAiCacheDto?> GetAsync(
        DateTime weekStart, 
        DateTime weekEnd, 
        string productType,
        string modelType,
        CancellationToken cancellationToken = default);
    
    Task<ExecAiCacheDto> SaveAsync(
        DateTime weekStart,
        DateTime weekEnd,
        string productType,
        string modelType,
        string summaryJson,
        int? userId,
        CancellationToken cancellationToken = default);
    
    Task<ExecAiCacheDto> UpdateAsync(
        int id,
        string summaryJson,
        int? userId,
        CancellationToken cancellationToken = default);
}