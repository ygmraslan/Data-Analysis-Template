using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.ExecSummary.Dtos;

namespace DataAnalysis.Application.Features.ExecSummary.Abstractions;

public interface IAiSummaryService
{
    Task<ExecAiDto> GenerateStructuredSummaryAsync(
        AiSummaryDataDto data,
        CancellationToken cancellationToken = default);

    Task<ModelResponseDto> GenerateForSingleModelAsync(
        AiSummaryDataDto data,
        AiModelType modelType,
        CancellationToken cancellationToken = default);
}