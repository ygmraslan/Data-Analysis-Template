using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.City.Queries.GetCityTrend;

public class GetCityTrendQuery : IRequest<List<CityTrendResponse>>, IFilteredQuery
{
    public ProductGroup ProductGroup { get; set; }
    public string City { get; set; } = string.Empty;
    public DetailFilter Filter { get; set; } = new();
}