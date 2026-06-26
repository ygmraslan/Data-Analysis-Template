namespace DataAnalysis.Application.Features.ExecSummary.Dtos;
public class WeekRangeDto
{

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string DisplayText { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public int WeekIndex { get; set; }
}