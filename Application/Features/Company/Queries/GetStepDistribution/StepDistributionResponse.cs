namespace DataAnalysis.Application.Features.Company.Queries.GetStepDistribution;

public class StepDistributionResponse
{
    public string Week { get; set; } = string.Empty;
    public int Step { get; set; }
    public int PolicyCount { get; set; }
}