namespace DataAnalysis.Application.Features.Dashboard.Queries.GetKpi;

public class GetKpiQueryResponse
{
    public int WeeklyPolicyCount { get; set; }
    public decimal WeeklyNetPremium { get; set; }
    public decimal ZeroStepRatio { get; set; }
    public decimal PolicyWoW { get; set; }
    public decimal NetPremiumWoW { get; set; }
    public decimal ZeroStepWoW { get; set; }
    public int PrevWeeklyPolicyCount { get; set; }
    public decimal PrevWeeklyNetPremium { get; set; }
}