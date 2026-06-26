using System.Text.Json.Serialization;

namespace DataAnalysis.Application.Features.Vehicle.Queries.GetVehiclePrice;

public class VehiclePriceResponse
{
    public string PriceRange { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    public decimal AvgPremium { get; set; }
    [JsonPropertyName("wow")]
    public decimal WoW { get; set; }
}