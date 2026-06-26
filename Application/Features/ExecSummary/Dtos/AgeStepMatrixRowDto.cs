namespace DataAnalysis.Application.Features.ExecSummary.Dtos;

public class AgeStepMatrixRowDto
{
    public string AgeGroup { get; set; } = string.Empty;
    public int Step0 { get; set; }
    public int Step1 { get; set; }
    public int Step2 { get; set; }
    public int Step3 { get; set; }
    public int Step4Plus { get; set; }
    public int Total { get; set; }
}