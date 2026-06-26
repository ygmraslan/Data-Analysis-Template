namespace DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleHeatmap;

public class VehicleHeatmapResponse
{
    public string Label { get; set; } = string.Empty;
    public string Week { get; set; } = string.Empty;
    public decimal AvgNetPremium { get; set; }
    public decimal PolicyRatio { get; set; }
}