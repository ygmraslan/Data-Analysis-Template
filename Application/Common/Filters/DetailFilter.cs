using DataAnalysis.Application.Common.Enums;

namespace DataAnalysis.Application.Common.Filters;

public sealed class DetailFilter
{
    public IReadOnlyList<InsuredType>    InsuredTypes    { get; init; } = [];
    public IReadOnlyList<BusinessSource> BusinessSources { get; init; } = [];
    public IReadOnlyList<VehicleType>    VehicleTypes    { get; init; } = [];
    public IReadOnlyList<string>         ProductCodes    { get; init; } = [];

    public bool IsEmpty =>
        InsuredTypes.Count == 0 && BusinessSources.Count == 0 &&
        VehicleTypes.Count == 0 && ProductCodes.Count == 0;
}