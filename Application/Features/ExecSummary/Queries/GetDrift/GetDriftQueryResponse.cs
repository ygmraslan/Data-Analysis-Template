using DataAnalysis.Application.Features.ExecSummary.Dtos;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetDrift;

public class GetDriftQueryResponse
{
    public List<DriftWeekDto> WeeklyTrend { get; set; } = new();
    public decimal Seg1StartShare { get; set; }
    public decimal Seg1EndShare { get; set; }
    public decimal Seg1GrowthMultiple { get; set; }
    public decimal Seg2StartShare { get; set; }
    public decimal Seg2EndShare { get; set; }
    public decimal Seg2GrowthMultiple { get; set; }
}