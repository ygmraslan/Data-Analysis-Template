using DataAnalysis.API.Filters;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyKpi;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyList;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyTrend;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyProfile;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyRegion;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyHeatmap;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysis.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class AgencyController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IExcelExportService _excelExportService;
    private readonly IPdfExportService _pdfExportService;

    public AgencyController(IMediator mediator, IExcelExportService excelExportService, IPdfExportService pdfExportService)
    {
        _mediator           = mediator;
        _excelExportService = excelExportService;
        _pdfExportService   = pdfExportService;
    }

    [HttpGet("kpi")]
    [Authorize(Policy = "Agency.Kpi.View")]
    public async Task<IActionResult> GetKpi([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetAgencyKpiQuery(productGroup) { Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("list")]
    [Authorize(Policy = "Agency.List.View")]
    public async Task<IActionResult> GetList(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? region = null,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetAgencyListQuery(productGroup, page, pageSize, region) { Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("trend")]
    [Authorize(Policy = "Agency.Trend.View")]
    public async Task<IActionResult> GetTrend(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] string agencyCode = "",
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetAgencyTrendQuery(productGroup, agencyCode) { Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("profile")]
    [Authorize(Policy = "Agency.Profile.View")]
    public async Task<IActionResult> GetProfile(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] string agencyCode = "",
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetAgencyProfileQuery(productGroup, agencyCode) { Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("region")]
    [Authorize(Policy = "Agency.Region.View")]
    public async Task<IActionResult> GetRegion([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetAgencyRegionQuery(productGroup) { Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("heatmap")]
    [Authorize(Policy = "Agency.Heatmap.View")]
    public async Task<IActionResult> GetHeatmap(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetAgencyHeatmapQuery(productGroup, page, pageSize) { Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("export/heatmap")]
    [Authorize(Policy = "Agency.Heatmap.Export")]
    public async Task<IActionResult> ExportHeatmap(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var domainFilter = (filter ?? new()).ToDomain();
        var summary      = FilterSummary.Build(domainFilter, productGroup);

        var data     = await _mediator.Send(new GetAgencyHeatmapQuery(productGroup, 1, 100) { Filter = domainFilter });
        var bytes    = _excelExportService.BuildAgencyHeatmapExport([data], productGroup.ToString(), summary);
        var fileName = $"AcenteHeatmap_{productGroup}_{DateTime.Today:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet("export/report")]
    [Authorize(Policy = "Agency.Report.Export")]
    public async Task<IActionResult> ExportReport(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] string agencyCode = "",
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var domainFilter = (filter ?? new()).ToDomain();
        var summary      = FilterSummary.Build(domainFilter, productGroup);

        var kpi     = await _mediator.Send(new GetAgencyKpiQuery(productGroup) { Filter = domainFilter });
        var list    = await _mediator.Send(new GetAgencyListQuery(productGroup, 1, 50) { Filter = domainFilter });
        var region  = await _mediator.Send(new GetAgencyRegionQuery(productGroup) { Filter = domainFilter });
        var heatmap = await _mediator.Send(new GetAgencyHeatmapQuery(productGroup, 1, 50) { Filter = domainFilter });

        var selectedAgency = !string.IsNullOrEmpty(agencyCode) 
            ? agencyCode 
            : list.Items.FirstOrDefault()?.AgencyCode ?? "";

        var trend   = await _mediator.Send(new GetAgencyTrendQuery(productGroup, selectedAgency) { Filter = domainFilter });
        var profile = await _mediator.Send(new GetAgencyProfileQuery(productGroup, selectedAgency) { Filter = domainFilter });

        var selectedAgencyName = list.Items.FirstOrDefault(x => x.AgencyCode == selectedAgency)?.AgencyName ?? "";
        var topRegion = region.Items.OrderByDescending(r => r.NetPremium).FirstOrDefault()?.Region ?? "";
        var regionAgencies = await _mediator.Send(new GetAgencyListQuery(productGroup, 1, 10, topRegion) { Filter = domainFilter });

        var bytes    = _pdfExportService.BuildAgencyReport(kpi, [list], [trend], [region], profile, [heatmap], productGroup.ToString(), selectedAgencyName, topRegion, regionAgencies, summary);
        var fileName = $"AcenteRaporu_{productGroup}_{DateTime.Today:yyyyMMdd}.pdf";
        return File(bytes, "application/pdf", fileName);
    }
}