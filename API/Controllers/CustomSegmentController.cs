using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using DataAnalysis.Application.Features.CustomSegment.Commands.DeleteComparison;
using DataAnalysis.Application.Features.CustomSegment.Commands.DeleteSegment;
using DataAnalysis.Application.Features.CustomSegment.Commands.RunComparison;
using DataAnalysis.Application.Features.CustomSegment.Commands.RunSegment;
using DataAnalysis.Application.Features.CustomSegment.Commands.SaveComparison;
using DataAnalysis.Application.Features.CustomSegment.Commands.SaveSegment;
using DataAnalysis.Application.Features.CustomSegment.Queries.CalculateDrift;
using DataAnalysis.Application.Features.CustomSegment.Queries.GetCalculateAi;
using DataAnalysis.Application.Features.CustomSegment.Queries.GetCompareAi;
using DataAnalysis.Application.Features.CustomSegment.Queries.GetComparisonById;
using DataAnalysis.Application.Features.CustomSegment.Queries.GetComparisons;
using DataAnalysis.Application.Features.CustomSegment.Queries.GetOptions;
using DataAnalysis.Application.Features.CustomSegment.Queries.GetSegmentById;
using DataAnalysis.Application.Features.CustomSegment.Queries.GetSegmentHistory;
using DataAnalysis.Application.Features.CustomSegment.Queries.GetSegments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysis.API.Controllers;

[Authorize]
[Route("api/custom-segment")]
public class CustomSegmentController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICustomSegmentDbRepository _dbRepository;
    private readonly ICustomSegmentAiService _aiService;

    public CustomSegmentController(
        IMediator mediator,
        ICustomSegmentDbRepository dbRepository,
        ICustomSegmentAiService aiService)
    {
        _mediator = mediator;
        _dbRepository = dbRepository;
        _aiService = aiService;
    }

    [HttpGet("options")]
    [Authorize(Policy = "CustomSegment.View")]
    public async Task<IActionResult> GetOptions([FromQuery] ProductGroup productGroup = ProductGroup.KASKO)
    {
        var result = await _mediator.Send(new GetOptionsQuery { ProductGroup = productGroup });
        return Ok(result);
    }

    [HttpPost("calculate")]
    [Authorize(Policy = "CustomSegment.View")]
    public async Task<IActionResult> CalculateDrift([FromBody] CalculateDriftQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Policy = "CustomSegment.View")]
    public async Task<IActionResult> GetSegments(
        [FromQuery] ProductGroup? productGroup = null,
        [FromQuery] string? search = null)
    {
        var result = await _mediator.Send(new GetSegmentsQuery
        {
            ProductGroup = productGroup,
            Search = search
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "CustomSegment.View")]
    public async Task<IActionResult> GetSegmentById(int id)
    {
        var result = await _mediator.Send(new GetSegmentByIdQuery { Id = id });
        if (result == null)
            return NotFound(new { message = "Segment bulunamadı." });
        return Ok(result);
    }

    [HttpGet("{id}/history")]
    [Authorize(Policy = "CustomSegment.View")]
    public async Task<IActionResult> GetSegmentHistory(int id)
    {
        var result = await _mediator.Send(new GetSegmentHistoryQuery { SegmentId = id });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "CustomSegment.Create")]
    public async Task<IActionResult> SaveSegment([FromBody] SaveSegmentCommand command)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized(new { message = "Kullanıcı kimliği bulunamadı." });

        command.UserId = userId.Value;
        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(result);
    }

    [HttpPost("{id}/run")]
    [Authorize(Policy = "CustomSegment.Run")]
    public async Task<IActionResult> RunSegment(int id, [FromBody] RunSegmentCommand command)
    {
        command.SegmentId = id;
        command.UserId = GetUserId();
        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "CustomSegment.Delete")]
    public async Task<IActionResult> DeleteSegment(int id)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized(new { message = "Kullanıcı kimliği bulunamadı." });

        var result = await _mediator.Send(new DeleteSegmentCommand
        {
            SegmentId = id,
            UserId = userId.Value
        });

        if (!result.Success)
            return NotFound(new { message = result.Message });

        return Ok(result);
    }

    [HttpGet("comparisons")]
    [Authorize(Policy = "CustomSegment.View")]
    public async Task<IActionResult> GetComparisons([FromQuery] string? productGroup = null)
    {
        var result = await _mediator.Send(new GetComparisonsQuery { ProductGroup = productGroup });
        return Ok(result);
    }

    [HttpGet("comparisons/{id}")]
    [Authorize(Policy = "CustomSegment.View")]
    public async Task<IActionResult> GetComparisonById(int id)
    {
        var result = await _mediator.Send(new GetComparisonByIdQuery { Id = id });
        if (result == null)
            return NotFound(new { message = "Karşılaştırma bulunamadı." });
        return Ok(result);
    }

    [HttpPost("comparisons")]
    [Authorize(Policy = "CustomSegment.Create")]
    public async Task<IActionResult> SaveComparison([FromBody] SaveComparisonCommand command)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized(new { message = "Kullanıcı kimliği bulunamadı." });

        command.UserId = userId.Value;
        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(result);
    }

    [HttpDelete("comparisons/{id}")]
    [Authorize(Policy = "CustomSegment.Delete")]
    public async Task<IActionResult> DeleteComparison(int id)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized(new { message = "Kullanıcı kimliği bulunamadı." });

        var result = await _mediator.Send(new DeleteComparisonCommand
        {
            Id = id,
            UserId = userId.Value
        });

        if (!result.Success)
            return NotFound(new { message = result.Message });

        return Ok(result);
    }

    [HttpPost("comparisons/{id}/run")]
    [Authorize(Policy = "CustomSegment.Run")]
    public async Task<IActionResult> RunComparison(int id, [FromBody] RunComparisonCommand command)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized(new { message = "Kullanıcı kimliği bulunamadı." });

        command.ComparisonId = id;
        command.UserId = userId.Value;

        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(result);
    }

    [HttpPost("calculate/ai/{model}")]
    [Authorize(Policy = "CustomSegment.View")]
    public async Task<IActionResult> GetCalculateAiComment(
        string model,
        [FromBody] GetCalculateAiQuery query)
    {
        if (!Enum.TryParse<AiModelType>(model, true, out var modelType))
            return BadRequest(new { message = "Geçersiz model tipi. deepseek, gemini veya gpt kullanın." });

        query.ModelType = modelType;
        var result = await _mediator.Send(query);

        if (!result.Success)
            return BadRequest(new { message = result.Error });

        return Ok(new { comment = result.Comment });
    }

    [HttpPost("calculate-compare/ai/{model}")]
    [Authorize(Policy = "CustomSegment.View")]
    public async Task<IActionResult> GetCompareAiComment(
        string model,
        [FromBody] GetCompareAiQuery query)
    {
        if (!Enum.TryParse<AiModelType>(model, true, out var modelType))
            return BadRequest(new { message = "Geçersiz model tipi. deepseek, gemini veya gpt kullanın." });

        query.ModelType = modelType;
        var result = await _mediator.Send(query);

        if (!result.Success)
            return BadRequest(new { message = result.Error });

        return Ok(new { comment = result.Comment });
    }

    [HttpPost("{id}/ai/deepseek")]
    [Authorize(Policy = "CustomSegment.View")]
    public async Task<IActionResult> GetAiDeepSeek(int id, [FromQuery] int? resultId = null)
    {
        return await GetAiComment(id, resultId, AiModelType.DeepSeek);
    }

    [HttpPost("{id}/ai/gemini")]
    [Authorize(Policy = "CustomSegment.View")]
    public async Task<IActionResult> GetAiGemini(int id, [FromQuery] int? resultId = null)
    {
        return await GetAiComment(id, resultId, AiModelType.Gemini);
    }

    [HttpPost("{id}/ai/gpt")]
    [Authorize(Policy = "CustomSegment.View")]
    public async Task<IActionResult> GetAiGpt(int id, [FromQuery] int? resultId = null)
    {
        return await GetAiComment(id, resultId, AiModelType.Gpt);
    }

    [HttpPost("compare/ai/deepseek")]
    [Authorize(Policy = "CustomSegment.View")]
    public async Task<IActionResult> CompareAiDeepSeek([FromQuery] int segmentAId, [FromQuery] int segmentBId)
    {
        return await GetComparisonComment(segmentAId, segmentBId, AiModelType.DeepSeek);
    }

    [HttpPost("compare/ai/gemini")]
    [Authorize(Policy = "CustomSegment.View")]
    public async Task<IActionResult> CompareAiGemini([FromQuery] int segmentAId, [FromQuery] int segmentBId)
    {
        return await GetComparisonComment(segmentAId, segmentBId, AiModelType.Gemini);
    }

    [HttpPost("compare/ai/gpt")]
    [Authorize(Policy = "CustomSegment.View")]
    public async Task<IActionResult> CompareAiGpt([FromQuery] int segmentAId, [FromQuery] int segmentBId)
    {
        return await GetComparisonComment(segmentAId, segmentBId, AiModelType.Gpt);
    }

    private async Task<IActionResult> GetAiComment(int segmentId, int? resultId, AiModelType modelType)
    {
        var segment = await _dbRepository.GetByIdAsync(segmentId);
        if (segment == null)
            return NotFound(new { message = "Segment bulunamadı." });

        var result = segment.LastResult;
        if (resultId.HasValue)
        {
            var history = await _dbRepository.GetHistoryAsync(segmentId);
            result = history.FirstOrDefault(r => r.Id == resultId.Value);
        }

        if (result == null)
            return BadRequest(new { message = "Segment sonucu bulunamadı. Önce segment'i çalıştırın." });

        var cachedComment = modelType switch
        {
            AiModelType.DeepSeek => result.AiCommentDeepSeek,
            AiModelType.Gemini => result.AiCommentGemini,
            AiModelType.Gpt => result.AiCommentGpt,
            _ => null
        };

        if (!string.IsNullOrEmpty(cachedComment))
        {
            return Ok(new { comment = cachedComment, cached = true });
        }

        var comment = await _aiService.GenerateCommentAsync(segment, result, modelType);

        await _dbRepository.UpdateResultAiCommentsAsync(
            result.Id,
            modelType == AiModelType.DeepSeek ? comment : result.AiCommentDeepSeek,
            modelType == AiModelType.Gemini ? comment : result.AiCommentGemini,
            modelType == AiModelType.Gpt ? comment : result.AiCommentGpt);

        return Ok(new { comment = comment, cached = false });
    }

    private async Task<IActionResult> GetComparisonComment(int segmentAId, int segmentBId, AiModelType modelType)
    {
        var segmentA = await _dbRepository.GetByIdAsync(segmentAId);
        var segmentB = await _dbRepository.GetByIdAsync(segmentBId);

        if (segmentA == null || segmentB == null)
            return NotFound(new { message = "Segment bulunamadı." });

        if (segmentA.LastResult == null || segmentB.LastResult == null)
            return BadRequest(new { message = "Her iki segment'in de çalıştırılmış sonucu olmalı." });

        var comment = await _aiService.GenerateComparisonCommentAsync(
            segmentA, segmentA.LastResult,
            segmentB, segmentB.LastResult,
            modelType);

        return Ok(new { comment = comment });
    }
}