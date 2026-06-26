namespace DataAnalysis.Application.Features.Vehicle.Dtos;

public class VehicleTrendDto
{
    public string WeekLabel { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    public decimal WoW { get; set; }
}