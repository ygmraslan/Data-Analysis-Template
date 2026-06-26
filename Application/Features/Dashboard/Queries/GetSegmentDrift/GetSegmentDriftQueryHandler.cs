using DataAnalysis.Application.Features.Dashboard.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Dashboard.Queries.GetSegmentDrift;

public class GetSegmentDriftQueryHandler : IRequestHandler<GetSegmentDriftQuery, List<GetSegmentDriftQueryResponse>>
{
    private readonly IDashboardRepository _dashboardRepository;

    public GetSegmentDriftQueryHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<List<GetSegmentDriftQueryResponse>> Handle(GetSegmentDriftQuery request, CancellationToken cancellationToken)
    {
        var result = await _dashboardRepository.GetSegmentDriftAsync(request.ProductGroup, request.Filter, cancellationToken);

        return result.Select(x => new GetSegmentDriftQueryResponse
        {
            WeekStart = x.WeekStart,
            TotalPolicy = x.TotalPolicy,
            Seg1Share = x.Seg1Share,
            Seg2Share = x.Seg2Share,
            Seg1WoW = x.Seg1WoW,
            Seg2WoW = x.Seg2WoW,
            Seg1Rolling4 = x.Seg1Rolling4,
            Seg2Rolling4 = x.Seg2Rolling4
        }).ToList();
    }
}