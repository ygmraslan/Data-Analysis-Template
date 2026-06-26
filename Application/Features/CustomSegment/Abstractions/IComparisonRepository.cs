using DataAnalysis.Application.Features.CustomSegment.Dtos;

namespace DataAnalysis.Application.Features.CustomSegment.Abstractions;

public interface IComparisonRepository
{
    Task<List<ComparisonSummaryDto>> GetAllAsync(
        string? productGroup,
        CancellationToken cancellationToken = default);

    Task<ComparisonDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        SaveComparisonRequestDto request,
        int userId,
        CancellationToken cancellationToken = default);
    Task UpdateAiCommentsAsync(
        int comparisonId,
        string? deepSeek,
        string? gemini,
        string? gpt,
        int userId,
        CancellationToken cancellationToken = default);

    Task UpdateResultsAsync(
        int comparisonId,
        DateTime weekStart,
        DateTime weekEnd,
        ComparisonSideResultDto sideAResult,
        ComparisonSideResultDto sideBResult,
        int userId,
        CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(
        int id,
        int userId,
        CancellationToken cancellationToken = default);
}