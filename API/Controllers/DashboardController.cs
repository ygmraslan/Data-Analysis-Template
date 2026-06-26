using DataAnalysis.API.Filters;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.Dashboard.Queries.GetDistribution;
using DataAnalysis.Application.Features.Dashboard.Queries.GetHeatmap;
using DataAnalysis.Application.Features.Dashboard.Queries.GetKpi;
using DataAnalysis.Application.Features.Dashboard.Queries.GetSegmentDrift;
using DataAnalysis.Application.Features.Dashboard.Queries.GetWeeklyTotals;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysis.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class DashboardController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IExcelExportService _excelExportService;

    public DashboardController(IMediator mediator, IExcelExportService excelExportService)
    {
        _mediator = mediator;
        _excelExportService = excelExportService;
    }

    [HttpGet("kpi")]
    [Authorize(Policy = "Dashboard.Kpi.View")]
    public async Task<IActionResult> GetKpi([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetKpiQuery { ProductGroup = productGroup, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("weekly-totals")]
    [Authorize(Policy = "Dashboard.Kpi.View")]
    public async Task<IActionResult> GetWeeklyTotals([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetWeeklyTotalsQuery { ProductGroup = productGroup, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("segment-drift")]
    [Authorize(Policy = "Dashboard.SegmentDrift.View")]
    public async Task<IActionResult> GetSegmentDrift([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetSegmentDriftQuery { ProductGroup = productGroup, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("distribution")]
    [Authorize(Policy = "Dashboard.Distribution.View")]
    public async Task<IActionResult> GetDistribution(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DistributionType distributionType = DistributionType.Brand,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetDistributionQuery
        {
            ProductGroup = productGroup,
            DistributionType = distributionType,
            Filter = (filter ?? new()).ToDomain()
        });
        return Ok(result);
    }

    [HttpGet("heatmap")]
    [Authorize(Policy = "Dashboard.Heatmap.View")]
    public async Task<IActionResult> GetHeatmap([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetHeatmapQuery { ProductGroup = productGroup, Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("export/heatmap")]
    [Authorize(Policy = "Dashboard.Heatmap.Export")]
    public async Task<IActionResult> ExportHeatmap(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var domainFilter = (filter ?? new()).ToDomain();
        var summary      = FilterSummary.Build(domainFilter, productGroup);

        var data = await _mediator.Send(new GetHeatmapQuery { ProductGroup = productGroup, Filter = domainFilter });
        var bytes = _excelExportService.BuildHeatmapExport(data, productGroup.ToString(), summary);
        var fileName = $"heatmap_{productGroup}_{DateTime.Today:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}