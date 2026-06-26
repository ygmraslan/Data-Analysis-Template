using System.Text;
using Dapper;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;

namespace DataAnalysis.Infrastructure.Octopus;

/// <summary>
/// TEMPLATE: Builds the dynamic WHERE clause (and its parameters) that every repository
/// appends to its base query, based on the active <see cref="DetailFilter"/>.
///
/// The original implementation mapped each filter dimension to a concrete source column.
/// Those column names have been removed. For each dimension below, replace
/// "YOUR_*_COLUMN" with the actual column in your source table, then keep using
/// parameterized values (never string-concatenate user input into SQL).
/// </summary>
public static class FilterSqlBuilder
{
    public sealed record Result(string Where, DynamicParameters Parameters);

    public static Result Build(DetailFilter filter, ProductGroup group)
    {
        var sb = new StringBuilder();
        var p = new DynamicParameters();

        // Dimension 1: maps filter.InsuredTypes to the column storing the entity/record type.
        if (filter.InsuredTypes.Count > 0)
        {
            sb.Append(" AND TRIM(YOUR_TYPE_COLUMN) IN @fInsuredTypes");
            p.Add("fInsuredTypes", filter.InsuredTypes.Select(x => x.ToDbValue()).ToArray());
        }

        // Dimension 2: maps filter.BusinessSources to the column storing the source/channel.
        if (filter.BusinessSources.Count > 0)
        {
            sb.Append(" AND TRIM(YOUR_SOURCE_COLUMN) IN @fBusinessSources");
            p.Add("fBusinessSources", filter.BusinessSources.Select(x => x.ToDbValue()).ToArray());
        }

        // Dimension 3: maps filter.VehicleTypes to the column storing the item/category type.
        if (filter.VehicleTypes.Count > 0)
        {
            sb.Append(" AND TRIM(YOUR_CATEGORY_COLUMN) IN @fVehicleTypes");
            p.Add("fVehicleTypes", filter.VehicleTypes.Select(x => x.ToDbValue()).ToArray());
        }

        // Dimension 4: maps filter.ProductCodes (resolved per product group via FilterCatalog)
        // to the column storing the product/sub-product code.
        if (filter.ProductCodes.Count > 0)
        {
            var dbValues = filter.ProductCodes
                .Select(code => FilterCatalog.TryGetProductDbValue(group, code, out var v) ? v : null)
                .Where(v => !string.IsNullOrEmpty(v))
                .ToArray();

            if (dbValues.Length > 0)
            {
                sb.Append(" AND TRIM(YOUR_PRODUCT_COLUMN) IN @fProductCodes");
                p.Add("fProductCodes", dbValues);
            }
        }

        return new Result(sb.ToString(), p);
    }
}
