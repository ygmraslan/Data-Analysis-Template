using System.ComponentModel.DataAnnotations;

namespace DataAnalysis.Application.Features.ExecSummary.Dtos;

public class ExecRequestDto
{
    [Required]
    public DateTime WeekStart { get; set; }
    
    [Required]
    public DateTime WeekEnd { get; set; }
    
    [Required]
    public string ProductType { get; set; } = "KASKO";
    public bool ForceRefresh { get; set; } = false;
}