namespace DataAnalysis.Application.Common.Filters;

public sealed record FilterOptionItem(string Value, string Label);
public sealed record ProductOptionItem(string Code, string Label);

public sealed class FilterOptionsResponse
{
    public List<FilterOptionItem> InsuredType    { get; set; } = new();
    public List<FilterOptionItem> BusinessSource { get; set; } = new();
    public List<FilterOptionItem> VehicleType    { get; set; } = new();
    public Dictionary<string, List<ProductOptionItem>> Product { get; set; } = new(); 
}