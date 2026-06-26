namespace DataAnalysis.Application.Features.City.Queries.GetCityProfile;

public class CityTopBrandResponse
{
    public string Brand { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
}

public class CityProfileItemResponse
{
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
}

public class CityProfileResponse
{
    public List<CityTopBrandResponse> TopBrands { get; set; } = [];
    public List<CityProfileItemResponse> Profile { get; set; } = [];
}