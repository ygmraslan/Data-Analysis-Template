using DataAnalysis.Application.Features.Agency.Queries.GetAgencyHeatmap;
using DataAnalysis.Application.Features.Brand.Queries.GetBrandHeatmap;
using DataAnalysis.Application.Features.City.Queries.GetCityHeatmap;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyHeatmap;
using DataAnalysis.Application.Features.Dashboard.Queries.GetHeatmap;
using DataAnalysis.Application.Features.Region.Queries.GetRegionHeatmap;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleHeatmap;


namespace DataAnalysis.Application.Common.Interfaces;

public interface IExcelExportService
{
    byte[] BuildHeatmapExport(List<GetHeatmapQueryResponse> data, string productGroup, string filterSummary = "");
    byte[] BuildRegionHeatmapExport(List<GetRegionHeatmapQueryResponse> data, string productGroup, string filterSummary = "");
    byte[] BuildBrandHeatmapExport(List<BrandHeatmapResponse> data, string productGroup, string filterSummary = "");
    byte[] BuildCityHeatmapExport(List<CityHeatmapResponse> data, string productGroup, string filterSummary = "");
    byte[] BuildVehicleHeatmapExport(List<VehicleHeatmapResponse> data, string rowHeader, string productGroup, string filterSummary = "");
    byte[] BuildCompanyHeatmapExport(List<GetCompanyHeatmapResponse> data, string productGroup, string filterSummary = "");
    byte[] BuildAgencyHeatmapExport(List<GetAgencyHeatmapResponse> data, string productGroup, string filterSummary = "");
}