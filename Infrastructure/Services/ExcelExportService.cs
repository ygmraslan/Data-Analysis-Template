using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyHeatmap;
using DataAnalysis.Application.Features.Brand.Queries.GetBrandHeatmap;
using DataAnalysis.Application.Features.City.Queries.GetCityHeatmap;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyHeatmap;
using DataAnalysis.Application.Features.Dashboard.Queries.GetHeatmap;
using DataAnalysis.Application.Features.Region.Queries.GetRegionHeatmap;
using DataAnalysis.Application.Features.Region.Queries.GetRegionKpi;
using DataAnalysis.Application.Features.Region.Queries.GetRegionTrend;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleHeatmap;
using DataAnalysis.Infrastructure.Excel;

namespace DataAnalysis.Infrastructure.Services;

public class ExcelExportService : IExcelExportService
{
    public byte[] BuildHeatmapExport(List<GetHeatmapQueryResponse> data, string productGroup, string filterSummary = "")
        => new HeatmapExcelExport().Build(data, productGroup, filterSummary);

    public byte[] BuildRegionHeatmapExport(List<GetRegionHeatmapQueryResponse> data, string productGroup, string filterSummary = "")
        => new RegionHeatmapExcelExport().Build(data, productGroup, filterSummary);

    public byte[] BuildBrandHeatmapExport(List<BrandHeatmapResponse> data, string productGroup, string filterSummary = "")
        => new BrandHeatmapExcelExport().Build(data, productGroup, filterSummary);

    public byte[] BuildCityHeatmapExport(List<CityHeatmapResponse> data, string productGroup, string filterSummary = "")
        => new CityHeatmapExcelExport().Build(data, productGroup, filterSummary);

    public byte[] BuildVehicleHeatmapExport(List<VehicleHeatmapResponse> data, string rowHeader, string productGroup, string filterSummary = "")
        => new VehicleHeatmapExcelExport().Build(data, rowHeader, productGroup, filterSummary);

    public byte[] BuildCompanyHeatmapExport(List<GetCompanyHeatmapResponse> data, string productGroup, string filterSummary = "")
        => new CompanyHeatmapExcelExport().Build(data, productGroup, filterSummary);

    public byte[] BuildAgencyHeatmapExport(List<GetAgencyHeatmapResponse> data, string productGroup, string filterSummary = "")
        => new AgencyHeatmapExcelExport().Build(data, productGroup, filterSummary);
}