using DataAnalysis.API.Filters;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleHeatmap;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleTrend;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleKpi;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleAge;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehiclePrice;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleBody;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleSegment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysis.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class VehicleController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IExcelExportService _excelExportService;
    private readonly IPdfExportService _pdfExportService;

    public VehicleController(IMediator mediator, IExcelExportService excelExportService, IPdfExportService pdfExportService)
    {
        _mediator           = mediator;
        _excelExportService = excelExportService;
        _pdfExportService   = pdfExportService;
    }

    [HttpGet("kpi")]
    [Authorize(Policy = "Vehicle.Kpi.View")]
    public async Task<IActionResult> GetKpi([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetVehicleKpiQuery { ProductGroup = productGroup, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("age")]
    [Authorize(Policy = "Vehicle.Age.View")]
    public async Task<IActionResult> GetAge(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] bool grouped = true,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetVehicleAgeQuery { ProductGroup = productGroup, Grouped = grouped, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("price")]
    [Authorize(Policy = "Vehicle.Price.View")]
    public async Task<IActionResult> GetPrice([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetVehiclePriceQuery { ProductGroup = productGroup, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("body")]
    [Authorize(Policy = "Vehicle.Body.View")]
    public async Task<IActionResult> GetBody([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetVehicleBodyQuery { ProductGroup = productGroup, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("segment")]
    [Authorize(Policy = "Vehicle.Segment.View")]
    public async Task<IActionResult> GetSegment([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetVehicleSegmentQuery { ProductGroup = productGroup, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("trend")]
    [Authorize(Policy = "Vehicle.Age.View")]
    public async Task<IActionResult> GetTrend(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] VehicleTrendType trendType = VehicleTrendType.Age,
        [FromQuery] string group = "",
        [FromQuery] bool grouped = true,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetVehicleTrendQuery { ProductGroup = productGroup, TrendType = trendType, Group = group, Grouped = grouped, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("heatmap/age")]
    [Authorize(Policy = "Vehicle.Heatmap.View")]
    public async Task<IActionResult> GetAgeHeatmap([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetVehicleHeatmapQuery { ProductGroup = productGroup, HeatmapType = VehicleHeatmapType.Age, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("heatmap/price")]
    [Authorize(Policy = "Vehicle.Heatmap.View")]
    public async Task<IActionResult> GetPriceHeatmap([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetVehicleHeatmapQuery { ProductGroup = productGroup, HeatmapType = VehicleHeatmapType.Price, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("export/heatmap/age")]
    [Authorize(Policy = "Vehicle.Heatmap.Export")]
    public async Task<IActionResult> ExportAgeHeatmap(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var domainFilter = (filter ?? new()).ToDomain();
        var summary      = FilterSummary.Build(domainFilter, productGroup);

        var data     = await _mediator.Send(new GetVehicleHeatmapQuery { ProductGroup = productGroup, HeatmapType = VehicleHeatmapType.Age, Filter = domainFilter });
        var bytes    = _excelExportService.BuildVehicleHeatmapExport(data, "Araç Yaşı", productGroup.ToString(), summary);
        var fileName = $"AracYasiHeatmap_{productGroup}_{DateTime.Today:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet("export/heatmap/price")]
    [Authorize(Policy = "Vehicle.Heatmap.Export")]
    public async Task<IActionResult> ExportPriceHeatmap(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var domainFilter = (filter ?? new()).ToDomain();
        var summary      = FilterSummary.Build(domainFilter, productGroup);

        var data     = await _mediator.Send(new GetVehicleHeatmapQuery { ProductGroup = productGroup, HeatmapType = VehicleHeatmapType.Price, Filter = domainFilter });
        var bytes    = _excelExportService.BuildVehicleHeatmapExport(data, "Araç Bedeli", productGroup.ToString(), summary);
        var fileName = $"AracBedeliHeatmap_{productGroup}_{DateTime.Today:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet("export/report")]
    [Authorize(Policy = "Vehicle.Report.Export")]
    public async Task<IActionResult> ExportReport(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var domainFilter = (filter ?? new()).ToDomain();
        var summary      = FilterSummary.Build(domainFilter, productGroup);

        var kpi          = await _mediator.Send(new GetVehicleKpiQuery    { ProductGroup = productGroup, Filter = domainFilter });
        var age          = await _mediator.Send(new GetVehicleAgeQuery    { ProductGroup = productGroup, Grouped = true, Filter = domainFilter });
        var price        = await _mediator.Send(new GetVehiclePriceQuery  { ProductGroup = productGroup, Filter = domainFilter });
        var body         = await _mediator.Send(new GetVehicleBodyQuery   { ProductGroup = productGroup, Filter = domainFilter });
        var segment      = await _mediator.Send(new GetVehicleSegmentQuery { ProductGroup = productGroup, Filter = domainFilter });
        var ageHeatmap   = await _mediator.Send(new GetVehicleHeatmapQuery { ProductGroup = productGroup, HeatmapType = VehicleHeatmapType.Age, Filter = domainFilter });
        var priceHeatmap = await _mediator.Send(new GetVehicleHeatmapQuery { ProductGroup = productGroup, HeatmapType = VehicleHeatmapType.Price, Filter = domainFilter });
        var ageTrend     = await _mediator.Send(new GetVehicleTrendQuery   { ProductGroup = productGroup, TrendType = VehicleTrendType.Age,   Group = kpi.TopGainerAge   ?? string.Empty, Filter = domainFilter });
        var priceTrend   = await _mediator.Send(new GetVehicleTrendQuery   { ProductGroup = productGroup, TrendType = VehicleTrendType.Price, Group = kpi.TopGainerPrice ?? string.Empty, Filter = domainFilter });

        var bytes    = _pdfExportService.BuildVehicleReport(kpi, age, price, body, segment, ageHeatmap, priceHeatmap, ageTrend, priceTrend, productGroup.ToString(), summary);
        var fileName = $"AracRaporu_{productGroup}_{DateTime.Today:yyyyMMdd}.pdf";
        return File(bytes, "application/pdf", fileName);
    }
}