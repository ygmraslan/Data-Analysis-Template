using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using DataAnalysis.Application.Features.CustomSegment.Dtos;
using DataAnalysis.Domain.Entities.CustomSegment;
using DataAnalysis.Infrastructure.Common;
using DataAnalysis.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAnalysis.Infrastructure.Repositories;

public class CustomSegmentDbRepository : ICustomSegmentDbRepository
{
    private readonly AppDbContext _context;

    public CustomSegmentDbRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SegmentDto>> GetAllAsync(
        ProductGroup? productGroup,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _context.CustomSegments
            .Include(x => x.CreatedByUser)
            .Include(x => x.Results.Where(r => !r.IsDeleted))
                .ThenInclude(r => r.CreatedByUser)
            .AsQueryable();

        if (productGroup.HasValue)
        {
            query = query.Where(x => x.ProductGroup == productGroup.Value.ToString());
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Name.Contains(search));
        }

        var segments = await query
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        return segments.Select(MapToSegmentDto).ToList();
    }

    public async Task<SegmentDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var segment = await _context.CustomSegments
            .Include(x => x.CreatedByUser)
            .Include(x => x.Results.Where(r => !r.IsDeleted))
                .ThenInclude(r => r.CreatedByUser)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return segment == null ? null : MapToSegmentDto(segment);
    }

    public async Task<SegmentDto> CreateAsync(
        SegmentDto dto,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var entity = new CustomSegment
        {
            Name = dto.Name,
            ProductGroup = dto.ProductGroup,
            Brands = JsonHelper.Serialize(dto.Filters.Brands),
            InsuredAges = JsonHelper.Serialize(dto.Filters.InsuredAges),
            InsuredTypes = JsonHelper.Serialize(dto.Filters.InsuredTypes),
            Genders = JsonHelper.Serialize(dto.Filters.Genders),
            VehicleAges = JsonHelper.Serialize(dto.Filters.VehicleAges),
            VehicleValues = JsonHelper.Serialize(dto.Filters.VehicleValues),
            CreatedBy = userId
        };

        await _context.CustomSegments.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToSegmentDto(entity);
    }

    public async Task DeleteAsync(
        int id,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var segment = await _context.CustomSegments
            .Include(x => x.Results)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (segment == null)
            return;

        segment.IsDeleted = true;
        segment.DeletedDate = DateTime.UtcNow;
        segment.DeletedBy = userId;

        foreach (var result in segment.Results)
        {
            result.IsDeleted = true;
            result.DeletedDate = DateTime.UtcNow;
            result.DeletedBy = userId;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<SegmentResultDto> AddResultAsync(
        int segmentId,
        SegmentResultDto dto,
        int? userId,
        CancellationToken cancellationToken = default)
    {
        var entity = new CustomSegmentResult
        {
            SegmentId = segmentId,
            StartDate = DateTime.SpecifyKind(dto.StartDate.Date, DateTimeKind.Utc),
            EndDate = DateTime.SpecifyKind(dto.EndDate.Date, DateTimeKind.Utc),
            TotalPolicy = dto.TotalPolicy,
            SegmentCount = dto.SegmentCount,
            StartShare = dto.StartShare,
            EndShare = dto.EndShare,
            Change = dto.Change,
            GrowthMultiple = dto.GrowthMultiple,
            WeeklyData = JsonHelper.Serialize(dto.WeeklyData) ?? "[]",
            AiCommentDeepSeek = dto.AiCommentDeepSeek,
            AiCommentGemini = dto.AiCommentGemini,
            AiCommentGpt = dto.AiCommentGpt,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = userId
        };

        await _context.CustomSegmentResults.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        dto.Id = entity.Id;
        dto.CreatedDate = entity.CreatedDate;
        return dto;
    }

    public async Task<SegmentResultDto?> GetCachedResultAsync(
        int segmentId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var utcStart = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
        var utcEnd = DateTime.SpecifyKind(endDate.Date, DateTimeKind.Utc);

        var result = await _context.CustomSegmentResults
            .AsNoTracking()
            .Include(x => x.CreatedByUser)
            .FirstOrDefaultAsync(x =>
                x.SegmentId == segmentId &&
                x.StartDate == utcStart &&
                x.EndDate == utcEnd,
                cancellationToken);

        return result == null ? null : MapToResultDto(result);
    }

    public async Task<List<SegmentResultDto>> GetHistoryAsync(
        int segmentId,
        CancellationToken cancellationToken = default)
    {
        var results = await _context.CustomSegmentResults
            .AsNoTracking()
            .Include(x => x.CreatedByUser)
            .Where(x => x.SegmentId == segmentId)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        return results.Select(MapToResultDto).ToList();
    }

    public async Task UpdateResultAiCommentsAsync(
        int resultId,
        string? deepSeekComment,
        string? geminiComment,
        string? gptComment,
        CancellationToken cancellationToken = default)
    {
        var result = await _context.CustomSegmentResults
            .FirstOrDefaultAsync(x => x.Id == resultId, cancellationToken);

        if (result != null)
        {
            result.AiCommentDeepSeek = deepSeekComment;
            result.AiCommentGemini = geminiComment;
            result.AiCommentGpt = gptComment;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    #region Mapping

    private static SegmentDto MapToSegmentDto(CustomSegment entity)
    {
        var lastResult = entity.Results?
            .OrderByDescending(r => r.CreatedDate)
            .FirstOrDefault();

        return new SegmentDto
        {
            Id = entity.Id,
            Name = entity.Name,
            ProductGroup = entity.ProductGroup,
            Filters = new SegmentFilterDto
            {
                Brands = JsonHelper.DeserializeStringList(entity.Brands),
                InsuredAges = JsonHelper.DeserializeStringList(entity.InsuredAges),
                InsuredTypes = JsonHelper.DeserializeStringList(entity.InsuredTypes),
                Genders = JsonHelper.DeserializeStringList(entity.Genders),
                VehicleAges = JsonHelper.DeserializeStringList(entity.VehicleAges),
                VehicleValues = JsonHelper.DeserializeStringList(entity.VehicleValues)
            },
            CreatedDate = entity.CreatedDate,
            CreatedByName = entity.CreatedByUser != null
                ? $"{entity.CreatedByUser.FirstName} {entity.CreatedByUser.LastName}"
                : null,
            ResultCount = entity.Results?.Count ?? 0,
            LastResult = lastResult != null ? MapToResultDto(lastResult) : null
        };
    }

    private static SegmentResultDto MapToResultDto(CustomSegmentResult entity)
    {
        return new SegmentResultDto
        {
            Id = entity.Id,
            SegmentId = entity.SegmentId,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            TotalPolicy = entity.TotalPolicy,
            SegmentCount = entity.SegmentCount,
            StartShare = entity.StartShare,
            EndShare = entity.EndShare,
            Change = entity.Change,
            GrowthMultiple = entity.GrowthMultiple,
            WeeklyData = JsonHelper.Deserialize<List<SegmentDriftWeekDto>>(entity.WeeklyData) ?? new(),
            AiCommentDeepSeek = entity.AiCommentDeepSeek,
            AiCommentGemini = entity.AiCommentGemini,
            AiCommentGpt = entity.AiCommentGpt,
            CreatedDate = entity.CreatedDate,
            CreatedByName = entity.CreatedByUser != null
                ? $"{entity.CreatedByUser.FirstName} {entity.CreatedByUser.LastName}"
                : null
        };
    }

    #endregion
}