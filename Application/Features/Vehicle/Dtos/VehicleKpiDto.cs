namespace DataAnalysis.Application.Features.Vehicle.Dtos;

public class VehicleKpiDto
{
    public string TopGainerAge { get; set; } = string.Empty;
    public decimal TopGainerAgeWoW { get; set; }
    public bool HasAgeGainer { get; set; }
    public string PrevTopGainerAge { get; set; } = string.Empty;
    public decimal PrevTopGainerAgeWoW { get; set; }

    public string TopLoserAge { get; set; } = string.Empty;
    public decimal TopLoserAgeWoW { get; set; }
    public bool HasAgeLoser { get; set; }
    public string PrevTopLoserAge { get; set; } = string.Empty;
    public decimal PrevTopLoserAgeWoW { get; set; }

    public string TopGainerPrice { get; set; } = string.Empty;
    public decimal TopGainerPriceWoW { get; set; }
    public bool HasPriceGainer { get; set; }
    public string PrevTopGainerPrice { get; set; } = string.Empty;
    public decimal PrevTopGainerPriceWoW { get; set; }

    public string TopLoserPrice { get; set; } = string.Empty;
    public decimal TopLoserPriceWoW { get; set; }
    public bool HasPriceLoser { get; set; }
    public string PrevTopLoserPrice { get; set; } = string.Empty;
    public decimal PrevTopLoserPriceWoW { get; set; }

    public string DefaultAgeGroup { get; set; } = string.Empty;
    public string DefaultPriceRange { get; set; } = string.Empty;
}