using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using DataAnalysis.Application.Features.CustomSegment.Dtos;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetCalculateAi;

public class GetCalculateAiQueryHandler : IRequestHandler<GetCalculateAiQuery, GetCalculateAiQueryResponse>
{
    private readonly ICustomSegmentAiService _aiService;

    public GetCalculateAiQueryHandler(ICustomSegmentAiService aiService)
    {
        _aiService = aiService;
    }

    public async Task<GetCalculateAiQueryResponse> Handle(GetCalculateAiQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var segment = new SegmentDto
            {
                Name = "Anlık Hesaplama",
                ProductGroup = request.ProductGroup,
                Filters = request.Filters ?? new SegmentFilterDto()
            };

            var result = new SegmentResultDto
            {
                StartDate = request.WeekStart,
                EndDate = request.WeekEnd,
                TotalPolicy = request.Result.TotalPolicy,
                SegmentCount = request.Result.SegmentCount,
                StartShare = request.Result.StartShare,
                EndShare = request.Result.EndShare,
                Change = request.Result.Change,
                GrowthMultiple = request.Result.GrowthMultiple,
                WeeklyData = request.Result.WeeklyData
            };

            var comment = await _aiService.GenerateCommentAsync(segment, result, request.ModelType, cancellationToken);

            return new GetCalculateAiQueryResponse
            {
                Comment = comment,
                ModelName = request.ModelType.ToString(),
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new GetCalculateAiQueryResponse
            {
                Comment = string.Empty,
                ModelName = request.ModelType.ToString(),
                Success = false,
                Error = ex.Message
            };
        }
    }
}