using DataAnalysis.API.Filters;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyKpi;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyList;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyTrend;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyRenewal;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyProfile;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyHeatmap;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataAnalysis.Application.Features.Company.Queries.GetStepDistribution;

namespace DataAnalysis.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class CompanyController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IExcelExportService _excelExportService;
    private readonly IPdfExportService _pdfExportService;

    public CompanyController(IMediator mediator, IExcelExportService excelExportService, IPdfExportService pdfExportService)
    {
        _mediator           = mediator;
        _excelExportService = excelExportService;
        _pdfExportService   = pdfExportService;
    }

    [HttpGet("kpi")]
    [Authorize(Policy = "Company.Kpi.View")]
    public async Task<IActionResult> GetKpi([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetCompanyKpiQuery(productGroup) { Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("list")]
    [Authorize(Policy = "Company.List.View")]
    public async Task<IActionResult> GetList([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetCompanyListQuery(productGroup) { Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("trend")]
    [Authorize(Policy = "Company.Trend.View")]
    public async Task<IActionResult> GetTrend(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] string company = "",
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetCompanyTrendQuery(productGroup, company) { Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("renewal")]
    [Authorize(Policy = "Company.Renewal.View")]
    public async Task<IActionResult> GetRenewal([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetCompanyRenewalQuery(productGroup) { Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("renewal/steps")]
    [Authorize(Policy = "Company.Renewal.View")]
    public async Task<IActionResult> GetStepDistribution(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] string renewalType = "Yeni İş",
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetStepDistributionQuery(productGroup, renewalType) { Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("profile")]
    [Authorize(Policy = "Company.Profile.View")]
    public async Task<IActionResult> GetProfile(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] string company = "",
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetCompanyProfileQuery(productGroup, company) { Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("heatmap")]
    [Authorize(Policy = "Company.Heatmap.View")]
    public async Task<IActionResult> GetHeatmap([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetCompanyHeatmapQuery(productGroup) { Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("export/heatmap")]
    [Authorize(Policy = "Company.Heatmap.Export")]
    public async Task<IActionResult> ExportHeatmap(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var domainFilter = (filter ?? new()).ToDomain();
        var summary      = FilterSummary.Build(domainFilter, productGroup);

        var data     = await _mediator.Send(new GetCompanyHeatmapQuery(productGroup) { Filter = domainFilter });
        var bytes    = _excelExportService.BuildCompanyHeatmapExport(data, productGroup.ToString(), summary);
        var fileName = $"SirketGecisHeatmap_{productGroup}_{DateTime.Today:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet("export/report")]
    [Authorize(Policy = "Company.Report.Export")]
    public async Task<IActionResult> ExportReport(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var domainFilter = (filter ?? new()).ToDomain();
        var summary      = FilterSummary.Build(domainFilter, productGroup);

        var kpi     = await _mediator.Send(new GetCompanyKpiQuery(productGroup) { Filter = domainFilter });
        var list    = await _mediator.Send(new GetCompanyListQuery(productGroup) { Filter = domainFilter });
        var trend   = await _mediator.Send(new GetCompanyTrendQuery(productGroup, kpi.DefaultCompany) { Filter = domainFilter });
        var renewal = await _mediator.Send(new GetCompanyRenewalQuery(productGroup) { Filter = domainFilter });
        var profile = await _mediator.Send(new GetCompanyProfileQuery(productGroup, kpi.DefaultCompany) { Filter = domainFilter });
        var heatmap = await _mediator.Send(new GetCompanyHeatmapQuery(productGroup) { Filter = domainFilter });

        var bytes    = _pdfExportService.BuildCompanyReport(kpi, list, trend, renewal, profile, heatmap, productGroup.ToString(), summary);
        var fileName = $"SirketGecisRaporu_{productGroup}_{DateTime.Today:yyyyMMdd}.pdf";
        return File(bytes, "application/pdf", fileName);
    }
}