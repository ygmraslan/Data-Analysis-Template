using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetComparisons;

public class GetComparisonsQuery : IRequest<GetComparisonsResponse>
{
    public string? ProductGroup { get; set; }
}