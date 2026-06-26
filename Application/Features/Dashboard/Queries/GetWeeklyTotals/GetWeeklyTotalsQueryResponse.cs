namespace DataAnalysis.Application.Features.Dashboard.Queries.GetWeeklyTotals;

public class GetWeeklyTotalsQueryResponse
{
    public string WeekLabel { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    public decimal PolicyWoW { get; set; }
    public decimal NetPremiumWoW { get; set; }
}