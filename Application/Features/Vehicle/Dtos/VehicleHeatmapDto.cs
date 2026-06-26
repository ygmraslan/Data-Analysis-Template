namespace DataAnalysis.Application.Features.Vehicle.Dtos;

public class VehicleHeatmapDto
{
    public string Label { get; set; } = string.Empty;
    public string Week { get; set; } = string.Empty;
    public decimal AvgNetPremium { get; set; }
    public decimal PolicyRatio { get; set; }
}