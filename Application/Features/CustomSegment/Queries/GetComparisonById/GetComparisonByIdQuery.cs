using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetComparisonById;

public class GetComparisonByIdQuery : IRequest<GetComparisonByIdResponse?>
{
    public int Id { get; set; }
}