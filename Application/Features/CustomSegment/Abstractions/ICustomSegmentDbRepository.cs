using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.CustomSegment.Dtos;

namespace DataAnalysis.Application.Features.CustomSegment.Abstractions;

public interface ICustomSegmentDbRepository
{
    Task<List<SegmentDto>> GetAllAsync(
        ProductGroup? productGroup, 
        string? search, 
        CancellationToken cancellationToken = default);
    
    Task<SegmentDto?> GetByIdAsync(
        int id, 
        CancellationToken cancellationToken = default);
    
    Task<SegmentDto> CreateAsync(
        SegmentDto segment, 
        int userId,
        CancellationToken cancellationToken = default);
    
    Task DeleteAsync(
        int id, 
        int userId, 
        CancellationToken cancellationToken = default);
    
    Task<SegmentResultDto> AddResultAsync(
        int segmentId,
        SegmentResultDto result, 
        int? userId,
        CancellationToken cancellationToken = default);
    
    Task<SegmentResultDto?> GetCachedResultAsync(
        int segmentId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
    
    Task<List<SegmentResultDto>> GetHistoryAsync(
        int segmentId, 
        CancellationToken cancellationToken = default);
    
    Task UpdateResultAiCommentsAsync(
        int resultId,
        string? deepSeekComment,
        string? geminiComment,
        string? gptComment,
        CancellationToken cancellationToken = default);
}