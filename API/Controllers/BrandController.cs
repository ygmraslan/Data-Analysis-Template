using DataAnalysis.API.Filters;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.Brand.Queries.GetBrandKpi;
using DataAnalysis.Application.Features.Brand.Queries.GetBrandList;
using DataAnalysis.Application.Features.Brand.Queries.GetBrandTrend;
using DataAnalysis.Application.Features.Brand.Queries.GetBrandModels;
using DataAnalysis.Application.Features.Brand.Queries.GetBrandHeatmap;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysis.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class BrandController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IExcelExportService _excelExportService;
    private readonly IPdfExportService _pdfExportService;

    public BrandController(IMediator mediator, IExcelExportService excelExportService, IPdfExportService pdfExportService)
    {
        _mediator = mediator;
        _excelExportService = excelExportService;
        _pdfExportService = pdfExportService;
    }

    [HttpGet("kpi")]
    [Authorize(Policy = "Brand.Kpi.View")]
    public async Task<IActionResult> GetKpi([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetBrandKpiQuery { ProductGroup = productGroup, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("list")]
    [Authorize(Policy = "Brand.List.View")]
    public async Task<IActionResult> GetList([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetBrandListQuery { ProductGroup = productGroup, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("trend")]
    [Authorize(Policy = "Brand.Trend.View")]
    public async Task<IActionResult> GetTrend(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] string brand = "",
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetBrandTrendQuery { ProductGroup = productGroup, Brand = brand, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("models")]
    [Authorize(Policy = "Brand.Models.View")]
    public async Task<IActionResult> GetModels(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] string brand = "",
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetBrandModelsQuery { ProductGroup = productGroup, Brand = brand, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("heatmap")]
    [Authorize(Policy = "Brand.Heatmap.View")]
    public async Task<IActionResult> GetHeatmap([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetBrandHeatmapQuery { ProductGroup = productGroup, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("export/heatmap")]
    [Authorize(Policy = "Brand.Heatmap.Export")]
    public async Task<IActionResult> ExportHeatmap(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var domainFilter = (filter ?? new()).ToDomain();
        var summary      = FilterSummary.Build(domainFilter, productGroup);

        var data     = await _mediator.Send(new GetBrandHeatmapQuery { ProductGroup = productGroup, Filter = domainFilter });
        var bytes    = _excelExportService.BuildBrandHeatmapExport(data, productGroup.ToString(), summary);
        var fileName = $"MarkaHeatmap_{productGroup}_{DateTime.Today:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet("export/report")]
    [Authorize(Policy = "Brand.Report.Export")]
    public async Task<IActionResult> ExportReport(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var domainFilter = (filter ?? new()).ToDomain();
        var summary      = FilterSummary.Build(domainFilter, productGroup);

        var kpi     = await _mediator.Send(new GetBrandKpiQuery { ProductGroup = productGroup, Filter = domainFilter });
        var brand   = kpi.DefaultBrand;
        var trend   = await _mediator.Send(new GetBrandTrendQuery  { ProductGroup = productGroup, Brand = brand, Filter = domainFilter });
        var models  = await _mediator.Send(new GetBrandModelsQuery { ProductGroup = productGroup, Brand = brand, Filter = domainFilter });
        var heatmap = await _mediator.Send(new GetBrandHeatmapQuery { ProductGroup = productGroup, Filter = domainFilter });

        var bytes    = _pdfExportService.BuildBrandReport(kpi, trend, models, heatmap, productGroup.ToString(), brand, summary);
        var fileName = $"MarkaRaporu_{brand}_{productGroup}_{DateTime.Today:yyyyMMdd}.pdf";
        return File(bytes, "application/pdf", fileName);
    }
}