using DataAnalysis.Application.Features.ExecSummary.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetDrift;

public class GetDriftQueryHandler : IRequestHandler<GetDriftQuery, GetDriftQueryResponse>
{
    private readonly IExecSummaryRepository _repository;

    public GetDriftQueryHandler(IExecSummaryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetDriftQueryResponse> Handle(GetDriftQuery request, CancellationToken cancellationToken)
    {
        // Seçilen haftadan geriye 10 hafta hesapla
        // EndDate = seçilen haftanın Pazar günü (dahil)
        // StartDate = 10 hafta öncesinin Pazartesi günü
        var endDate = request.EndDate;
        var startDate = endDate.AddDays(-69); // 10 hafta = 70 gün, -69 ile 70 gün aralık
        
        var weeklyData = await _repository.GetDriftAsync(
            request.ProductGroup,
            startDate,
            endDate,
            cancellationToken);

        if (weeklyData.Count == 0)
            return new GetDriftQueryResponse();

        var firstWeek = weeklyData.First();
        var lastWeek = weeklyData.Last();

        return new GetDriftQueryResponse
        {
            WeeklyTrend = weeklyData,
            Seg1StartShare = firstWeek.Seg1Share,
            Seg1EndShare = lastWeek.Seg1Share,
            Seg1GrowthMultiple = firstWeek.Seg1Share > 0 
                ? Math.Round(lastWeek.Seg1Share / firstWeek.Seg1Share, 1) 
                : 0,
            Seg2StartShare = firstWeek.Seg2Share,
            Seg2EndShare = lastWeek.Seg2Share,
            Seg2GrowthMultiple = firstWeek.Seg2Share > 0 
                ? Math.Round(lastWeek.Seg2Share / firstWeek.Seg2Share, 1) 
                : 0
        };
    }
}