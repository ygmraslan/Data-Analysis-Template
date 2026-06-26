using DataAnalysis.API.Filters;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.City.Queries.GetCityKpi;
using DataAnalysis.Application.Features.City.Queries.GetCityList;
using DataAnalysis.Application.Features.City.Queries.GetCityTrend;
using DataAnalysis.Application.Features.City.Queries.GetCityProfile;
using DataAnalysis.Application.Features.City.Queries.GetCityHeatmap;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysis.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class CityController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IExcelExportService _excelExportService;
    private readonly IPdfExportService _pdfExportService;

    public CityController(IMediator mediator,IExcelExportService excelExportService, IPdfExportService pdfExportService)
    {
        _mediator          = mediator;
        _excelExportService = excelExportService;
        _pdfExportService  = pdfExportService;
    }

    [HttpGet("kpi")]
    [Authorize(Policy = "City.Kpi.View")]
    public async Task<IActionResult> GetKpi([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetCityKpiQuery { ProductGroup = productGroup, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("list")]
    [Authorize(Policy = "City.List.View")]
    public async Task<IActionResult> GetList([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetCityListQuery { ProductGroup = productGroup, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("trend")]
    [Authorize(Policy = "City.Trend.View")]
    public async Task<IActionResult> GetTrend(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] string city = "",
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetCityTrendQuery { ProductGroup = productGroup, City = city, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("profile")]
    [Authorize(Policy = "City.Profile.View")]
    public async Task<IActionResult> GetProfile(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] string city = "",
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetCityProfileQuery { ProductGroup = productGroup, City = city, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("heatmap")]
    [Authorize(Policy = "City.Heatmap.View")]
    public async Task<IActionResult> GetHeatmap([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetCityHeatmapQuery { ProductGroup = productGroup, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("export/heatmap")]
    [Authorize(Policy = "City.Heatmap.Export")]
    public async Task<IActionResult> ExportHeatmap(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var domainFilter = (filter ?? new()).ToDomain();
        var summary      = FilterSummary.Build(domainFilter, productGroup);

        var data     = await _mediator.Send(new GetCityHeatmapQuery { ProductGroup = productGroup, Filter = domainFilter });
        var bytes    = _excelExportService.BuildCityHeatmapExport(data, productGroup.ToString(), summary);
        var fileName = $"IlHeatmap_{productGroup}_{DateTime.Today:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet("export/report")]
    [Authorize(Policy = "City.Report.Export")]
    public async Task<IActionResult> ExportReport(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var domainFilter = (filter ?? new()).ToDomain();
        var summary      = FilterSummary.Build(domainFilter, productGroup);

        var kpi     = await _mediator.Send(new GetCityKpiQuery     { ProductGroup = productGroup, Filter = domainFilter });
        var list    = await _mediator.Send(new GetCityListQuery    { ProductGroup = productGroup, Filter = domainFilter });
        var trend   = await _mediator.Send(new GetCityTrendQuery   { ProductGroup = productGroup, City = kpi.DefaultCity, Filter = domainFilter });
        var profile = await _mediator.Send(new GetCityProfileQuery { ProductGroup = productGroup, City = kpi.DefaultCity, Filter = domainFilter });
        var heatmap = await _mediator.Send(new GetCityHeatmapQuery { ProductGroup = productGroup, Filter = domainFilter });

        var bytes    = _pdfExportService.BuildCityReport(kpi, list, trend, profile, heatmap, productGroup.ToString(), summary);
        var fileName = $"IlRaporu_{productGroup}_{DateTime.Today:yyyyMMdd}.pdf";
        return File(bytes, "application/pdf", fileName);
    }
}