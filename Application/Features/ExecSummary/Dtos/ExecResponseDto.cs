namespace DataAnalysis.Application.Features.ExecSummary.Dtos;

public class ExecResponseDto
{
    public WeekRangeDto SelectedWeek { get; set; } = new();
    public string ProductType { get; set; } = string.Empty;
    public bool HasData { get; set; }
    
    public DriftDataDto Drift { get; set; } = new();
    public MatrixDto<BrandAgeMatrixDto> BrandAge { get; set; } = new();
    public MatrixDto<AgeStepDto> AgeStep { get; set; } = new();
    public MatrixDto<YoungDriverDto> YoungDriver { get; set; } = new();
    public ListDto<RiskSegmentDto> RiskSegments { get; set; } = new();
    public DistributionsDto Distributions { get; set; } = new();
    public PortfolioDto Portfolio { get; set; } = new();
}

public class DriftDataDto
{
    public bool HasData { get; set; }
    public string Message { get; set; } = string.Empty;
    public SegmentDefDto Segment1 { get; set; } = new();
    public SegmentDefDto Segment2 { get; set; } = new();
    public List<DriftWeekDto> Trends { get; set; } = new();
    public DriftSummaryDto Summary1 { get; set; } = new();
    public DriftSummaryDto Summary2 { get; set; } = new();
}

public class SegmentDefDto
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<string> Criteria { get; set; } = new();
}

public class DriftSummaryDto
{
    public decimal StartShare { get; set; }
    public decimal EndShare { get; set; }
    public decimal Growth { get; set; }
    public string GrowthText { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}
public class MatrixDto<T>
{
    public bool HasData { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<T> Data { get; set; } = new();
    public string Comment { get; set; } = string.Empty;
}
public class ListDto<T>
{
    public bool HasData { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<T> Data { get; set; } = new();
}
public class AgeStepDto
{
    public string AgeGroup { get; set; } = string.Empty;
    public int Step0 { get; set; }
    public int Step1 { get; set; }
    public int Step2 { get; set; }
    public int Step3 { get; set; }
    public int Step4Plus { get; set; }
    public int Total { get; set; }
    public bool IsHighRisk { get; set; }
}

public class YoungDriverDto
{
    public string Brand { get; set; } = string.Empty;
    public int Count { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public bool IsPremium { get; set; }
}

public class DistributionsDto
{
    public bool HasData { get; set; }
    public DistItemsDto Brand { get; set; } = new();
    public DistItemsDto Age { get; set; } = new();
    public DistItemsDto Step { get; set; } = new();
    public List<RiskBandDto> InsuredAge { get; set; } = new();
}

public class DistItemsDto
{
    public List<DistItemDto> Items { get; set; } = new();
    public string Note { get; set; } = string.Empty;
}

public class DistItemDto
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percent { get; set; }
    public string Color { get; set; } = "blue";
}

public class RiskBandDto
{
    public string AgeRange { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class PortfolioDto
{
    public bool HasData { get; set; }
    public List<string> Characteristics { get; set; } = new();
    public List<string> RiskFactors { get; set; } = new();
    public List<string> PositiveFactors { get; set; } = new();
}