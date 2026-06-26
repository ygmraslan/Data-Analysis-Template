using DataAnalysis.API.Filters;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.Region.Queries.GetRegionHeatmap;
using DataAnalysis.Application.Features.Region.Queries.GetRegionKpi;
using DataAnalysis.Application.Features.Region.Queries.GetRegionTrend;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysis.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class RegionController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IExcelExportService _excelExportService;
    private readonly IPdfExportService _pdfExportService;

    public RegionController(IMediator mediator, IExcelExportService excelExportService, IPdfExportService pdfExportService)
    {
        _mediator = mediator;
        _excelExportService = excelExportService;
        _pdfExportService=pdfExportService;
    }

    [HttpGet("kpi")]
    [Authorize(Policy = "Region.Kpi.View")]
    public async Task<IActionResult> GetKpi(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetRegionKpiQuery
        {
            ProductGroup = productGroup,
            Filter = (filter ?? new()).ToDomain()
        });
        return Ok(result);
    }

    [HttpGet("trend")]
    [Authorize(Policy = "Region.Trend.View")]
    public async Task<IActionResult> GetTrend(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetRegionTrendQuery
        {
            ProductGroup = productGroup,
            Filter = (filter ?? new()).ToDomain()
        });
        return Ok(result);
    }

    [HttpGet("heatmap")]
    [Authorize(Policy = "Region.Heatmap.View")]
    public async Task<IActionResult> GetHeatmap(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetRegionHeatmapQuery
        {
            ProductGroup = productGroup,
            Filter = (filter ?? new()).ToDomain()
        });
        return Ok(result);
    }

    [HttpGet("export/heatmap")]
    [Authorize(Policy = "Region.Heatmap.Export")]
    public async Task<IActionResult> ExportHeatmap(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var domainFilter = (filter ?? new()).ToDomain();
        var summary      = FilterSummary.Build(domainFilter, productGroup);

        var data  = await _mediator.Send(new GetRegionHeatmapQuery { ProductGroup = productGroup, Filter = domainFilter });
        var bytes = _excelExportService.BuildRegionHeatmapExport(data, productGroup.ToString(), summary);
        var fileName = $"NetPrim_Bolge_{productGroup}_{DateTime.Today:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet("export/report")]
    [Authorize(Policy = "Region.Report.Export")]
    public async Task<IActionResult> ExportReport(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var domainFilter = (filter ?? new()).ToDomain();
        var summary      = FilterSummary.Build(domainFilter, productGroup);

        var kpi     = await _mediator.Send(new GetRegionKpiQuery    { ProductGroup = productGroup, Filter = domainFilter });
        var trend   = await _mediator.Send(new GetRegionTrendQuery   { ProductGroup = productGroup, Filter = domainFilter });
        var heatmap = await _mediator.Send(new GetRegionHeatmapQuery { ProductGroup = productGroup, Filter = domainFilter });

        var bytes    = _pdfExportService.BuildRegionReport(kpi, trend, heatmap, productGroup.ToString(), summary);
        var fileName = $"BolgeRaporu_{productGroup}_{DateTime.Today:yyyyMMdd}.pdf";
        return File(bytes, "application/pdf", fileName);
    }
}