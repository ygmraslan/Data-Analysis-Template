using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.ExecSummary.Dtos;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetAgeStep;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetAiSummary;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetBrandAge;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetDistribution;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetDrift;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetRisk;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetYoungDriver;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysis.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class ExecSummaryController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IPdfExportService _pdfExportService;
    private readonly IWeekCalculatorService _weekCalculatorService;

    public ExecSummaryController(
        IMediator mediator, 
        IPdfExportService pdfExportService,
        IWeekCalculatorService weekCalculatorService)
    {
        _mediator = mediator;
        _pdfExportService = pdfExportService;
        _weekCalculatorService = weekCalculatorService;
    }

    [HttpGet("available-weeks")]
    [Authorize(Policy = "ExecSummary.View")]
    public IActionResult GetAvailableWeeks([FromQuery] int year, [FromQuery] int month)
    {
        if (year < 2020 || year > 2100)
            return BadRequest(new { message = "Geçersiz yıl" });

        if (month < 1 || month > 12)
            return BadRequest(new { message = "Geçersiz ay" });

        var weeks = _weekCalculatorService.GetAvailableWeeks(year, month);
        return Ok(weeks);
    }

    [HttpGet("current-week")]
    [Authorize(Policy = "ExecSummary.View")]
    public IActionResult GetCurrentWeek()
    {
        var week = _weekCalculatorService.GetCurrentWeek();
        return Ok(week);
    }

    [HttpGet("drift")]
    [Authorize(Policy = "ExecSummary.View")]
    public async Task<IActionResult> GetDrift(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var result = await _mediator.Send(new GetDriftQuery
        {
            ProductGroup = productGroup,
            StartDate = start,
            EndDate = end
        });
        return Ok(result);
    }

    [HttpGet("brand-age")]
    [Authorize(Policy = "ExecSummary.View")]
    public async Task<IActionResult> GetBrandAge(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var result = await _mediator.Send(new GetBrandAgeQuery
        {
            ProductGroup = productGroup,
            StartDate = start,
            EndDate = end
        });
        return Ok(result);
    }

    [HttpGet("age-step")]
    [Authorize(Policy = "ExecSummary.View")]
    public async Task<IActionResult> GetAgeStep(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var result = await _mediator.Send(new GetAgeStepQuery
        {
            ProductGroup = productGroup,
            StartDate = start,
            EndDate = end
        });
        return Ok(result);
    }

    [HttpGet("young-driver")]
    [Authorize(Policy = "ExecSummary.View")]
    public async Task<IActionResult> GetYoungDriver(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var result = await _mediator.Send(new GetYoungDriverQuery
        {
            ProductGroup = productGroup,
            StartDate = start,
            EndDate = end
        });
        return Ok(result);
    }

    [HttpGet("risk")]
    [Authorize(Policy = "ExecSummary.View")]
    public async Task<IActionResult> GetRisk(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var result = await _mediator.Send(new GetRiskQuery
        {
            ProductGroup = productGroup,
            StartDate = start,
            EndDate = end
        });
        return Ok(result);
    }

    [HttpGet("distribution")]
    [Authorize(Policy = "ExecSummary.View")]
    public async Task<IActionResult> GetDistribution(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var result = await _mediator.Send(new GetDistributionQuery
        {
            ProductGroup = productGroup,
            StartDate = start,
            EndDate = end
        });
        return Ok(result);
    }

    [HttpPost("ai-summary")]
    [Authorize(Policy = "ExecSummary.View")]
    public async Task<IActionResult> GetAiSummary(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] AiModelType modelType = AiModelType.DeepSeek,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] bool forceRefresh = false)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var userId = GetUserId();
        
        var result = await _mediator.Send(new GetAiSummaryQuery
        {
            ProductGroup = productGroup,
            ModelType = modelType,
            StartDate = start,
            EndDate = end,
            ForceRefresh = forceRefresh,
            UserId = userId
        });
        return Ok(result);
    }

    [HttpPost("ai-deepseek")]
    [Authorize(Policy = "ExecSummary.View")]
    public async Task<IActionResult> GetAiDeepSeek(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] bool forceRefresh = false)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var userId = GetUserId();
        
        var result = await _mediator.Send(new GetAiSummaryQuery
        {
            ProductGroup = productGroup,
            ModelType = AiModelType.DeepSeek,
            StartDate = start,
            EndDate = end,
            ForceRefresh = forceRefresh,
            UserId = userId
        });
        return Ok(result);
    }

    [HttpPost("ai-gemini")]
    [Authorize(Policy = "ExecSummary.View")]
    public async Task<IActionResult> GetAiGemini(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] bool forceRefresh = false)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var userId = GetUserId();
        
        var result = await _mediator.Send(new GetAiSummaryQuery
        {
            ProductGroup = productGroup,
            ModelType = AiModelType.Gemini,
            StartDate = start,
            EndDate = end,
            ForceRefresh = forceRefresh,
            UserId = userId
        });
        return Ok(result);
    }

    [HttpPost("ai-gpt")]
    [Authorize(Policy = "ExecSummary.View")]
    public async Task<IActionResult> GetAiGpt(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] bool forceRefresh = false)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var userId = GetUserId();
        
        var result = await _mediator.Send(new GetAiSummaryQuery
        {
            ProductGroup = productGroup,
            ModelType = AiModelType.Gpt,
            StartDate = start,
            EndDate = end,
            ForceRefresh = forceRefresh,
            UserId = userId
        });
        return Ok(result);
    }

    [HttpGet("export/pdf")]
    [Authorize(Policy = "ExecSummary.Export")]
    public async Task<IActionResult> ExportPdf(
        [FromQuery] ProductGroup productGroup = ProductGroup.KASKO,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var (start, end) = GetDateRange(startDate, endDate);
 
        var driftTask = _mediator.Send(new GetDriftQuery { ProductGroup = productGroup, StartDate = start, EndDate = end });
        var brandAgeTask = _mediator.Send(new GetBrandAgeQuery { ProductGroup = productGroup, StartDate = start, EndDate = end });
        var ageStepTask = _mediator.Send(new GetAgeStepQuery { ProductGroup = productGroup, StartDate = start, EndDate = end });
        var youngDriverTask = _mediator.Send(new GetYoungDriverQuery { ProductGroup = productGroup, StartDate = start, EndDate = end });
        var riskTask = _mediator.Send(new GetRiskQuery { ProductGroup = productGroup, StartDate = start, EndDate = end });
        var distributionTask = _mediator.Send(new GetDistributionQuery { ProductGroup = productGroup, StartDate = start, EndDate = end });
        
        await Task.WhenAll(driftTask, brandAgeTask, ageStepTask, youngDriverTask, riskTask, distributionTask);

        var deepSeekResult = await _mediator.Send(new GetAiSummaryQuery { ProductGroup = productGroup, ModelType = AiModelType.DeepSeek, StartDate = start, EndDate = end });
        var geminiResult = await _mediator.Send(new GetAiSummaryQuery { ProductGroup = productGroup, ModelType = AiModelType.Gemini, StartDate = start, EndDate = end });
        var gptResult = await _mediator.Send(new GetAiSummaryQuery { ProductGroup = productGroup, ModelType = AiModelType.Gpt, StartDate = start, EndDate = end });

        var deepSeekData = deepSeekResult.Data.Success ? deepSeekResult.Data.Data : null;
        var geminiData = geminiResult.Data.Success ? geminiResult.Data.Data : null;
        var gptData = gptResult.Data.Success ? gptResult.Data.Data : null;
 
        var bytes = _pdfExportService.BuildExecSummaryPdf(
            productGroup.ToString(),
            start,
            end,
            driftTask.Result,
            brandAgeTask.Result,
            ageStepTask.Result,
            youngDriverTask.Result,
            riskTask.Result,
            distributionTask.Result,
            deepSeekData,
            geminiData,
            gptData);
 
        var fileName = $"Yonetici-Ozeti-{productGroup}-{DateTime.Today:yyyyMMdd}.pdf";
        return File(bytes, "application/pdf", fileName);
    }
 
    private static (DateTime start, DateTime end) GetDateRange(DateTime? startDate, DateTime? endDate)
    {
        var end = endDate ?? DateTime.Today;
        var start = startDate ?? end.AddDays(-70);
        return (start, end);
    }
}