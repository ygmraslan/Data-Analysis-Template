using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using DataAnalysis.Application.Features.CustomSegment.Dtos;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetCompareAi;

public class GetCompareAiHandler : IRequestHandler<GetCompareAiQuery, GetCompareAiResponse>
{
    private readonly ICustomSegmentAiService _aiService;

    public GetCompareAiHandler(ICustomSegmentAiService aiService)
    {
        _aiService = aiService;
    }

    public async Task<GetCompareAiResponse> Handle(GetCompareAiQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var segmentA = new SegmentDto
            {
                Name = "Segment A",
                ProductGroup = request.ProductGroup,
                Filters = request.FiltersA ?? new SegmentFilterDto()
            };

            var resultA = MapToResultDto(request.ResultA, request.WeekStart, request.WeekEnd);

            var segmentB = new SegmentDto
            {
                Name = "Segment B",
                ProductGroup = request.ProductGroup,
                Filters = request.FiltersB ?? new SegmentFilterDto()
            };

            var resultB = MapToResultDto(request.ResultB, request.WeekStart, request.WeekEnd);

            var comment = await _aiService.GenerateComparisonCommentAsync(
                segmentA, resultA,
                segmentB, resultB,
                request.ModelType,
                cancellationToken);

            return new GetCompareAiResponse
            {
                Comment = comment,
                ModelName = request.ModelType.ToString(),
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new GetCompareAiResponse
            {
                Comment = string.Empty,
                ModelName = request.ModelType.ToString(),
                Success = false,
                Error = ex.Message
            };
        }
    }

    private static SegmentResultDto MapToResultDto(SegmentDriftResultDto source, DateTime weekStart, DateTime weekEnd)
    {
        return new SegmentResultDto
        {
            StartDate = weekStart,
            EndDate = weekEnd,
            TotalPolicy = source.TotalPolicy,
            SegmentCount = source.SegmentCount,
            StartShare = source.StartShare,
            EndShare = source.EndShare,
            Change = source.Change,
            GrowthMultiple = source.GrowthMultiple,
            WeeklyData = source.WeeklyData
        };
    }
}