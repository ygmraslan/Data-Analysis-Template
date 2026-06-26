namespace DataAnalysis.Application.Features.Vehicle.Dtos;

public class VehicleBodyDto
{
    public string BodyType { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    public decimal WoW { get; set; }
}