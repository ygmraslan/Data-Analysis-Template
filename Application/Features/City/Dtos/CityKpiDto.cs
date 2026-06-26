namespace DataAnalysis.Application.Features.City.Dtos;

public class CityKpiDto
{
    public string TopCity { get; set; } = string.Empty;
    public decimal TopCityPremium { get; set; }
    public string TopCityPrev { get; set; } = string.Empty;
    public decimal TopCityPrevPremium { get; set; }

    public string BottomCity { get; set; } = string.Empty;
    public decimal BottomCityPremium { get; set; }
    public string BottomCityPrev { get; set; } = string.Empty;
    public decimal BottomCityPrevPremium { get; set; }

    public string TopGainerCity { get; set; } = string.Empty;
    public decimal TopGainerWoW { get; set; }
    public bool HasGainer { get; set; }
    public string PrevTopGainerCity { get; set; } = string.Empty;
    public decimal PrevTopGainerWoW { get; set; }

    public string TopLoserCity { get; set; } = string.Empty;
    public decimal TopLoserWoW { get; set; }
    public bool HasLoser { get; set; }
    public string PrevTopLoserCity { get; set; } = string.Empty;
    public decimal PrevTopLoserWoW { get; set; }

    public string DefaultCity { get; set; } = string.Empty;
}