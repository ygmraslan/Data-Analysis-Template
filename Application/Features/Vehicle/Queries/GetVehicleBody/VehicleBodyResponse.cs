using System.Text.Json.Serialization;

namespace DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleBody;

public class VehicleBodyResponse
{
    public string BodyType { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    [JsonPropertyName("wow")]
    public decimal WoW { get; set; }
}