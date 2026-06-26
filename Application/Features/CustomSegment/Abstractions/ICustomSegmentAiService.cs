using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.CustomSegment.Dtos;

namespace DataAnalysis.Application.Features.CustomSegment.Abstractions;

public interface ICustomSegmentAiService
{
    Task<string> GenerateCommentAsync(
        SegmentDto segment,
        SegmentResultDto result,
        AiModelType modelType,
        CancellationToken cancellationToken = default);

    Task<string> GenerateComparisonCommentAsync(
        SegmentDto segmentA,
        SegmentResultDto resultA,
        SegmentDto segmentB,
        SegmentResultDto resultB,
        AiModelType modelType,
        CancellationToken cancellationToken = default);
}