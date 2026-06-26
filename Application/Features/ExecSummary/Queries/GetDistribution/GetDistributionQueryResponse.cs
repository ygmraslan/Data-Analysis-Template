using DataAnalysis.Application.Features.ExecSummary.Dtos;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetDistribution;

public class GetDistributionQueryResponse
{
    public List<DistributionItemDto> Brands { get; set; } = new();
    public List<DistributionItemDto> VehicleAges { get; set; } = new();
    public List<DistributionItemDto> Steps { get; set; } = new();
    public List<DistributionItemDto> InsuredAges { get; set; } = new();
}