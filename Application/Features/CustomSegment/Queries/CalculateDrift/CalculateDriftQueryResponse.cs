namespace DataAnalysis.Application.Features.CustomSegment.Queries.CalculateDrift;

public class CalculateDriftQueryResponse
{
    public int TotalPolicy { get; set; }
    public int SegmentCount { get; set; }
    public decimal StartShare { get; set; }
    public decimal EndShare { get; set; }
    public decimal Change { get; set; }
    public decimal GrowthMultiple { get; set; }
    public List<CalculateDriftWeekItem> WeeklyData { get; set; } = new();
}

public class CalculateDriftWeekItem
{
    public DateTime WeekStart { get; set; }
    public string WeekLabel { get; set; } = string.Empty;
    public int TotalPolicy { get; set; }
    public int SegmentCount { get; set; }
    public decimal SegmentShare { get; set; }
    public decimal? WoW { get; set; }
}