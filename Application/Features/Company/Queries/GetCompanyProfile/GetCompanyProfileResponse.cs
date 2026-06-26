namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyProfile;

public class GetCompanyProfileResponse
{
    public List<TopBrandItem> TopBrands { get; set; } = new();
    public List<ProfileItem> Profile { get; set; } = new();
}

public class TopBrandItem
{
    public string Brand { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
}

public class ProfileItem
{
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
}