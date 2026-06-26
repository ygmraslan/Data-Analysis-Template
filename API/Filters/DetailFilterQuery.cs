using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysis.API.Filters;

public sealed class DetailFilterQuery
{
    [FromQuery(Name = "insuredType")]    public List<InsuredType>    InsuredTypes    { get; set; } = new();
    [FromQuery(Name = "businessSource")] public List<BusinessSource> BusinessSources { get; set; } = new();
    [FromQuery(Name = "vehicleType")]    public List<VehicleType>    VehicleTypes    { get; set; } = new();
    [FromQuery(Name = "product")]        public List<string>         Products        { get; set; } = new();

    public DetailFilter ToDomain() => new()
    {
        InsuredTypes    = InsuredTypes,
        BusinessSources = BusinessSources,
        VehicleTypes    = VehicleTypes,
        ProductCodes    = Products
    };
}