using DataAnalysis.Application.Common.Enums;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetOptions;

public class GetOptionsQuery : IRequest<GetOptionsQueryResponse>
{
    public ProductGroup ProductGroup { get; set; }
}