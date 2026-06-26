namespace DataAnalysis.Application.Features.Vehicle.Dtos;

public class VehicleSegmentDto
{
    public string Segment { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    public decimal WoW { get; set; }
}