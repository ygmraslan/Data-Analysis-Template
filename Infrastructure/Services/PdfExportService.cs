using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyHeatmap;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyKpi;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyList;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyProfile;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyRegion;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyTrend;
using DataAnalysis.Application.Features.Brand.Queries.GetBrandHeatmap;
using DataAnalysis.Application.Features.Brand.Queries.GetBrandKpi;
using DataAnalysis.Application.Features.Brand.Queries.GetBrandModels;
using DataAnalysis.Application.Features.Brand.Queries.GetBrandTrend;
using DataAnalysis.Application.Features.City.Queries.GetCityHeatmap;
using DataAnalysis.Application.Features.City.Queries.GetCityKpi;
using DataAnalysis.Application.Features.City.Queries.GetCityList;
using DataAnalysis.Application.Features.City.Queries.GetCityProfile;
using DataAnalysis.Application.Features.City.Queries.GetCityTrend;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyHeatmap;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyKpi;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyList;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyProfile;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyRenewal;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyTrend;
using DataAnalysis.Application.Features.Demographic.Queries.GetDemoDistribution;
using DataAnalysis.Application.Features.Demographic.Queries.GetDemoKpi;
using DataAnalysis.Application.Features.ExecSummary.Dtos;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetAgeStep;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetBrandAge;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetDistribution;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetDrift;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetRisk;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetYoungDriver;
using DataAnalysis.Application.Features.Region.Queries.GetRegionHeatmap;
using DataAnalysis.Application.Features.Region.Queries.GetRegionKpi;
using DataAnalysis.Application.Features.Region.Queries.GetRegionTrend;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleAge;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleBody;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleHeatmap;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleKpi;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehiclePrice;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleSegment;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleTrend;
using DataAnalysis.Infrastructure.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace DataAnalysis.Infrastructure.Services;

public class PdfExportService : IPdfExportService
{
    public PdfExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] BuildRegionReport(
        GetRegionKpiQueryResponse kpi,
        List<GetRegionTrendQueryResponse> trend,
        List<GetRegionHeatmapQueryResponse> heatmap,
        string productGroup,
        string filterSummary = "")
    {
        var dateRange = GetEightWeekRange();
        var document  = new RegionPdfReport(kpi, trend, heatmap, productGroup, dateRange, filterSummary);
        return document.GeneratePdf();
    }

    private static string GetEightWeekRange()
    {
        var today        = DateTime.Today;
        var daysToMonday = (int)today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1;
        var lastSun      = today.AddDays(-daysToMonday - 1);
        var eightWeekMon = lastSun.AddDays(-55);
        return $"{eightWeekMon:dd.MM} — {lastSun:dd.MM.yyyy}";
    }

    public byte[] BuildBrandReport(
        BrandKpiResponse kpi,
        List<BrandTrendResponse> trend,
        List<BrandModelResponse> models,
        List<BrandHeatmapResponse> heatmap,
        string productGroup,
        string brand,
        string filterSummary = "")
    {
        var dateRange = GetEightWeekRange();
        var document  = new BrandPdfReport(kpi, trend, models, heatmap, productGroup, brand, dateRange, filterSummary);
        return document.GeneratePdf();
    }

    public byte[] BuildCityReport(
        CityKpiResponse kpi,
        List<CityListResponse> list,
        List<CityTrendResponse> trend,
        CityProfileResponse profile,
        List<CityHeatmapResponse> heatmap,
        string productGroup,
        string filterSummary = "")
    {
        var dateRange = GetEightWeekRange();
        var document  = new CityPdfReport(kpi, list, trend, profile, heatmap, productGroup, dateRange, filterSummary);
        return document.GeneratePdf();
    }

    public byte[] BuildVehicleReport(
        VehicleKpiResponse kpi,
        List<VehicleAgeResponse> age,
        List<VehiclePriceResponse> price,
        List<VehicleBodyResponse> body,
        List<VehicleSegmentResponse> segment,
        List<VehicleHeatmapResponse> ageHeatmap,
        List<VehicleHeatmapResponse> priceHeatmap,
        List<VehicleTrendResponse> ageTrend,
        List<VehicleTrendResponse> priceTrend,
        string productGroup,
        string filterSummary = "")
    {
        var dateRange = GetEightWeekRange();
        var document  = new VehiclePdfReport(kpi, age, price, body, segment, ageHeatmap, priceHeatmap, ageTrend, priceTrend, productGroup, dateRange, filterSummary);
        return document.GeneratePdf();
    }

    public byte[] BuildCompanyReport(
        GetCompanyKpiResponse kpi,
        List<GetCompanyListResponse> list,
        List<GetCompanyTrendResponse> trend,
        List<GetCompanyRenewalResponse> renewal,
        GetCompanyProfileResponse profile,
        List<GetCompanyHeatmapResponse> heatmap,
        string productGroup,
        string filterSummary = "")
    {
        var today        = DateTime.Today;
        var daysToMonday = (int)today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1;
        var lastMon      = today.AddDays(-daysToMonday - 7);
        var lastSun      = lastMon.AddDays(6);
        var dateRange    = $"{lastMon:dd.MM} — {lastSun:dd.MM.yyyy}";

        var report = new CompanyPdfReport(kpi, list, trend, renewal, profile, heatmap, productGroup, dateRange, filterSummary);
        return report.GeneratePdf();
    }

    public byte[] BuildAgencyReport(
        GetAgencyKpiResponse kpi,
        List<GetAgencyListResponse> list,
        List<GetAgencyTrendResponse> trend,
        List<GetAgencyRegionResponse> region,
        GetAgencyProfileResponse profile,
        List<GetAgencyHeatmapResponse> heatmap,
        string productGroup,
        string selectedAgencyName = "",
        string topRegion = "",
        GetAgencyListResponse? regionAgencies = null,
        string filterSummary = "")
    {
        var today        = DateTime.Today;
        var daysToMonday = (int)today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1;
        var lastMon      = today.AddDays(-daysToMonday - 7);
        var lastSun      = lastMon.AddDays(6);
        var dateRange    = $"{lastMon:dd.MM} — {lastSun:dd.MM.yyyy}";

        var report = new AgencyPdfReport(kpi, list, trend, region, profile, heatmap, productGroup, dateRange, selectedAgencyName, topRegion, regionAgencies, filterSummary);
        return report.GeneratePdf();
    }

    public byte[] BuildDemoReport(
        GetDemoKpiResponse kpi,
        List<GetDemoDistributionResponse> insuredType,
        List<GetDemoDistributionResponse> gender,
        List<GetDemoDistributionResponse> ageGroup,
        List<GetDemoDistributionResponse> plateCity,
        string productGroup,
        string filterSummary = "")
    {
        var today        = DateTime.Today;
        var daysToMonday = (int)today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1;
        var lastMon      = today.AddDays(-daysToMonday - 7);
        var lastSun      = lastMon.AddDays(6);
        var dateRange    = $"{lastMon:dd.MM} — {lastSun:dd.MM.yyyy}";

        var report = new DemoPdfReport(kpi, insuredType, gender, ageGroup, plateCity, productGroup, dateRange, filterSummary);
        return report.GeneratePdf();
    }

    public byte[] BuildExecSummaryPdf(
        string productGroup,
        DateTime startDate,
        DateTime endDate,
        GetDriftQueryResponse drift,
        GetBrandAgeQueryResponse brandAge,
        GetAgeStepQueryResponse ageStep,
        GetYoungDriverQueryResponse youngDriver,
        GetRiskQueryResponse risk,
        GetDistributionQueryResponse distribution,
        ExecAiDto? deepSeekData,
        ExecAiDto? geminiData,
        ExecAiDto? gptData)
    {
        var dateRange = $"{startDate:dd.MM.yyyy} — {endDate:dd.MM.yyyy}";
        var report = new ExecSummaryPdfReport(
            productGroup, dateRange,
            drift, brandAge, ageStep, youngDriver, risk, distribution,
            deepSeekData, geminiData, gptData);
        return report.GeneratePdf();
    }
}