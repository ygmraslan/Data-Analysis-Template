using DataAnalysis.Application.Features.ExecSummary.Dtos;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetBrandAge;

public class GetBrandAgeQueryResponse
{
    public List<BrandAgeMatrixDto> Matrix { get; set; } = new();
}