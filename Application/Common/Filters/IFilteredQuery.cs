using DataAnalysis.Application.Common.Enums;
namespace DataAnalysis.Application.Common.Filters;

public interface IFilteredQuery
{
    ProductGroup ProductGroup { get; }
    DetailFilter Filter { get; }
}