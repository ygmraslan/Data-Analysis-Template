namespace DataAnalysis.Application.Features.ExecSummary.Dtos;

public class AiSummaryDataDto
{
    public List<DriftWeekDto> DriftTrend { get; set; } = new();
    public decimal Seg1StartShare { get; set; }
    public decimal Seg1EndShare { get; set; }
    public decimal Seg2StartShare { get; set; }
    public decimal Seg2EndShare { get; set; }
    public List<BrandAgeMatrixDto> BrandAgeMatrix { get; set; } = new();
    public List<AgeStepMatrixRowDto> AgeStepMatrix { get; set; } = new();

    public List<DistributionItemDto> BrandDistribution { get; set; } = new();
    public List<DistributionItemDto> VehicleAgeDistribution { get; set; } = new();
    public List<DistributionItemDto> StepDistribution { get; set; } = new();
    public List<DistributionItemDto> InsuredAgeDistribution { get; set; } = new();
    public List<DistributionItemDto> YoungDriverDistribution { get; set; } = new();

    public List<RiskSegmentDto> RiskSegments { get; set; } = new();
}