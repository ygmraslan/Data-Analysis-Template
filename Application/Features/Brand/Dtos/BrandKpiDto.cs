namespace DataAnalysis.Application.Features.Brand.Dtos;

public class BrandKpiDto
{
    public string TopBrand { get; set; } = string.Empty;
    public int TopBrandCount { get; set; }
    public string TopBrandPrev { get; set; } = string.Empty;
    public int TopBrandPrevCount { get; set; }

    public string BottomBrand { get; set; } = string.Empty;
    public int BottomBrandCount { get; set; }
    public string BottomBrandPrev { get; set; } = string.Empty;
    public int BottomBrandPrevCount { get; set; }

    public string TopGainerBrand { get; set; } = string.Empty;
    public decimal TopGainerWoW { get; set; }
    public bool HasGainer { get; set; }
    public string PrevTopGainerBrand { get; set; } = string.Empty;
    public decimal PrevTopGainerWoW { get; set; }

    public string TopLoserBrand { get; set; } = string.Empty;
    public decimal TopLoserWoW { get; set; }
    public bool HasLoser { get; set; }
    public string PrevTopLoserBrand { get; set; } = string.Empty;
    public decimal PrevTopLoserWoW { get; set; }

    public string DefaultBrand { get; set; } = string.Empty;
}