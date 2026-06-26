namespace DataAnalysis.Application.Features.Vehicle.Dtos;

public class VehiclePriceDto
{
    public string PriceRange { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    public decimal AvgPremium { get; set; }
    public decimal WoW { get; set; }
    public long? SortKey { get; set; }
}