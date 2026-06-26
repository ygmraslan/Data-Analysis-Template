using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Brand.Queries.GetBrandModels;

public class GetBrandModelsQuery : IRequest<List<BrandModelResponse>>, IFilteredQuery
{
    public ProductGroup ProductGroup { get; set; }
    public string Brand { get; set; } = string.Empty;
    public DetailFilter Filter { get; set; } = new();
}