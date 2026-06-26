using DataAnalysis.Application.Features.ExecSummary.Dtos;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetAgeStep;

public class GetAgeStepQueryResponse
{
    public List<AgeStepMatrixRowDto> Matrix { get; set; } = new();
}