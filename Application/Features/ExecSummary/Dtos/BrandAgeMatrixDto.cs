namespace DataAnalysis.Application.Features.ExecSummary.Dtos;

public class BrandAgeMatrixDto
{
    public string Brand { get; set; } = string.Empty;
    public int Age0To2 { get; set; }
    public int Age3To5 { get; set; }
    public int Age6To10 { get; set; }
    public int Age11To15 { get; set; }
    public int Age16Plus { get; set; }
    public int Total { get; set; }

    public bool IsPremiumBrand { get; set; }
}