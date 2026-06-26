using DataAnalysis.API.Filters;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.Demographic.Queries.GetDemoKpi;
using DataAnalysis.Application.Features.Demographic.Queries.GetDemoDistribution;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysis.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class DemoController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IPdfExportService _pdfExportService;

    public DemoController(IMediator mediator, IPdfExportService pdfExportService)
    {
        _mediator = mediator;
        _pdfExportService = pdfExportService;
    }

    [HttpGet("kpi")]
    [Authorize(Policy = "Demo.Kpi.View")]
    public async Task<IActionResult> GetKpi([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetDemoKpiQuery(productGroup) { Filter = (filter ?? new()).ToDomain() });
        return Ok(result);
    }

    [HttpGet("insured-type")]
    [Authorize(Policy = "Demo.InsuredType.View")]
    public async Task<IActionResult> GetInsuredType([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetDemoDistributionQuery
        {
            ProductGroup = productGroup,
            DistributionType = DemoDistributionType.InsuredType,
            Filter = (filter ?? new()).ToDomain()
        });
        return Ok(result);
    }

    [HttpGet("gender")]
    [Authorize(Policy = "Demo.Gender.View")]
    public async Task<IActionResult> GetGender([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetDemoDistributionQuery
        {
            ProductGroup = productGroup,
            DistributionType = DemoDistributionType.Gender,
            Filter = (filter ?? new()).ToDomain()
        });
        return Ok(result);
    }

    [HttpGet("age-group")]
    [Authorize(Policy = "Demo.AgeGroup.View")]
    public async Task<IActionResult> GetAgeGroup([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetDemoDistributionQuery
        {
            ProductGroup = productGroup,
            DistributionType = DemoDistributionType.AgeGroup,
            Filter = (filter ?? new()).ToDomain()
        });
        return Ok(result);
    }

    [HttpGet("insured-city")]
    [Authorize(Policy = "Demo.InsuredCity.View")]
    public async Task<IActionResult> GetInsuredCity([FromQuery] ProductGroup productGroup = ProductGroup.KASKO, [FromQuery] DetailFilterQuery? filter = null)
    {
        var result = await _mediator.Send(new GetDemoDistributionQuery
        {
            ProductGroup = productGroup,
            DistributionType = DemoDistributionType.InsuredCity,
            Filter = (filter ?? new()).ToDomain()
        });
        return Ok(result);
    }

    [HttpGet("export/report")]
    [Authorize(Policy = "Demo.Report.Export")]
    public async Task<IActionResult> ExportReport(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DetailFilterQuery? filter = null)
    {
        var domainFilter = (filter ?? new()).ToDomain();
        var summary      = FilterSummary.Build(domainFilter, productGroup);

        var kpiTask = _mediator.Send(new GetDemoKpiQuery(productGroup) { Filter = domainFilter });
        var insuredTypeTask = _mediator.Send(new GetDemoDistributionQuery { ProductGroup = productGroup, DistributionType = DemoDistributionType.InsuredType, Filter = domainFilter });
        var genderTask = _mediator.Send(new GetDemoDistributionQuery { ProductGroup = productGroup, DistributionType = DemoDistributionType.Gender, Filter = domainFilter });
        var ageGroupTask = _mediator.Send(new GetDemoDistributionQuery { ProductGroup = productGroup, DistributionType = DemoDistributionType.AgeGroup, Filter = domainFilter });
        var insuredCityTask = _mediator.Send(new GetDemoDistributionQuery { ProductGroup = productGroup, DistributionType = DemoDistributionType.InsuredCity, Filter = domainFilter });

        await Task.WhenAll(kpiTask, insuredTypeTask, genderTask, ageGroupTask, insuredCityTask);

        var pdf = _pdfExportService.BuildDemoReport(
            kpiTask.Result,
            insuredTypeTask.Result,
            genderTask.Result,
            ageGroupTask.Result,
            insuredCityTask.Result,
            productGroup.ToString(),
            summary);

        return File(pdf, "application/pdf", $"Demografik-Analiz-{productGroup}-{DateTime.Today:yyyyMMdd}.pdf");
    }
}