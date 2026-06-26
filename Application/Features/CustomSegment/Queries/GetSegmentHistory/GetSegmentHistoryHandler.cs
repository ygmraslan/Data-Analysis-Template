using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetSegmentHistory;

public class GetSegmentHistoryHandler : IRequestHandler<GetSegmentHistoryQuery, GetSegmentHistoryResponse>
{
    private readonly ICustomSegmentDbRepository _repository;

    public GetSegmentHistoryHandler(ICustomSegmentDbRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetSegmentHistoryResponse> Handle(GetSegmentHistoryQuery request, CancellationToken cancellationToken)
    {
        var results = await _repository.GetHistoryAsync(request.SegmentId, cancellationToken);

        var items = results.Select(r => new GetSegmentHistoryItem
        {
            Id = r.Id,
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            TotalPolicy = r.TotalPolicy,
            SegmentCount = r.SegmentCount,
            StartShare = r.StartShare,
            EndShare = r.EndShare,
            Change = r.Change,
            GrowthMultiple = r.GrowthMultiple,
            HasAiCommentDeepSeek = !string.IsNullOrEmpty(r.AiCommentDeepSeek),
            HasAiCommentGemini = !string.IsNullOrEmpty(r.AiCommentGemini),
            HasAiCommentGpt = !string.IsNullOrEmpty(r.AiCommentGpt),
            CreatedDate = r.CreatedDate
        }).ToList();

        return new GetSegmentHistoryResponse { Items = items };
    }
}