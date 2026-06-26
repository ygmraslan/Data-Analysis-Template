using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using DataAnalysis.Application.Features.CustomSegment.Dtos;
using DataAnalysis.Domain.Entities.CustomSegment;
using DataAnalysis.Infrastructure.Common;
using DataAnalysis.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAnalysis.Infrastructure.Repositories;

public class ComparisonRepository : IComparisonRepository
{
    private readonly AppDbContext _context;

    public ComparisonRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ComparisonSummaryDto>> GetAllAsync(
        string? productGroup,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ComparisonSegments
            .AsNoTracking()
            .Include(x => x.CreatedByUser)
            .Include(x => x.Results)
            .OrderByDescending(x => x.CreatedDate)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(productGroup))
        {
            query = query.Where(x => x.ProductGroup == productGroup);
        }

        var entities = await query.ToListAsync(cancellationToken);

        return entities.Select(MapToSummary).ToList();
    }

    public async Task<ComparisonDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.ComparisonSegments
            .AsNoTracking()
            .Include(x => x.CreatedByUser)
            .Include(x => x.Results)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? null : MapToDetail(entity);
    }

    public async Task<int> CreateAsync(
        SaveComparisonRequestDto request,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var comparison = new ComparisonSegment
        {
            Name = request.Name,
            ProductGroup = request.ProductGroup,
            WeekStart = DateTime.SpecifyKind(request.WeekStart.Date, DateTimeKind.Utc),
            WeekEnd = DateTime.SpecifyKind(request.WeekEnd.Date, DateTimeKind.Utc),
            AiCommentDeepSeek = request.AiCommentDeepSeek,
            AiCommentGemini = request.AiCommentGemini,
            AiCommentGpt = request.AiCommentGpt,
            CreatedBy = userId,
            CreatedDate = now
        };

        comparison.Results.Add(BuildResultEntity(
            ComparisonSide.A,
            request.SegmentAFilters,
            request.SegmentAResult,
            userId,
            now));

        comparison.Results.Add(BuildResultEntity(
            ComparisonSide.B,
            request.SegmentBFilters,
            request.SegmentBResult,
            userId,
            now));

        await _context.ComparisonSegments.AddAsync(comparison, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return comparison.Id;
    }

        public async Task UpdateAiCommentsAsync(
        int comparisonId,
        string? deepSeek,
        string? gemini,
        string? gpt,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.ComparisonSegments
            .FirstOrDefaultAsync(x => x.Id == comparisonId, cancellationToken);

        if (entity == null) return;

        if (deepSeek != null) entity.AiCommentDeepSeek = deepSeek;
        if (gemini != null) entity.AiCommentGemini = gemini;
        if (gpt != null) entity.AiCommentGpt = gpt;

        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task UpdateResultsAsync(
        int comparisonId,
        DateTime weekStart,
        DateTime weekEnd,
        ComparisonSideResultDto sideAResult,
        ComparisonSideResultDto sideBResult,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.ComparisonSegments
            .Include(x => x.Results)
            .FirstOrDefaultAsync(x => x.Id == comparisonId, cancellationToken);

        if (entity == null) return;

        var now = DateTime.UtcNow;

        // Parent: tarih aralığı güncelle, AI yorumları temizle
        entity.WeekStart = DateTime.SpecifyKind(weekStart.Date, DateTimeKind.Utc);
        entity.WeekEnd = DateTime.SpecifyKind(weekEnd.Date, DateTimeKind.Utc);
        entity.AiCommentDeepSeek = null;
        entity.AiCommentGemini = null;
        entity.AiCommentGpt = null;
        entity.UpdatedBy = userId;
        entity.UpdatedDate = now;

        var sideA = entity.Results.FirstOrDefault(r => r.Side == ComparisonSide.A);
        var sideB = entity.Results.FirstOrDefault(r => r.Side == ComparisonSide.B);

        if (sideA != null) ApplyResultToEntity(sideA, sideAResult, userId, now);
        if (sideB != null) ApplyResultToEntity(sideB, sideBResult, userId, now);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(
        int id,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.ComparisonSegments
            .Include(x => x.Results)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null) return false;

        var now = DateTime.UtcNow;

        entity.IsDeleted = true;
        entity.DeletedBy = userId;
        entity.DeletedDate = now;

        foreach (var result in entity.Results)
        {
            result.IsDeleted = true;
            result.DeletedBy = userId;
            result.DeletedDate = now;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ComparisonSegmentResult BuildResultEntity(
        ComparisonSide side,
        SegmentFilterDto filters,
        ComparisonSideResultDto result,
        int userId,
        DateTime now)
    {
        return new ComparisonSegmentResult
        {
            Side = side,
            Brands = JsonHelper.Serialize(filters.Brands),
            InsuredAges = JsonHelper.Serialize(filters.InsuredAges),
            InsuredTypes = JsonHelper.Serialize(filters.InsuredTypes),
            Genders = JsonHelper.Serialize(filters.Genders),
            VehicleAges = JsonHelper.Serialize(filters.VehicleAges),
            VehicleValues = JsonHelper.Serialize(filters.VehicleValues),
            TotalPolicy = result.TotalPolicy,
            SegmentCount = result.SegmentCount,
            StartShare = result.StartShare,
            EndShare = result.EndShare,
            Change = result.Change,
            GrowthMultiple = result.GrowthMultiple,
            WeeklyData = JsonHelper.Serialize(result.WeeklyData) ?? "[]",
            CreatedBy = userId,
            CreatedDate = now
        };
    }

    private static void ApplyResultToEntity(
        ComparisonSegmentResult entity,
        ComparisonSideResultDto result,
        int userId,
        DateTime now)
    {
        entity.TotalPolicy = result.TotalPolicy;
        entity.SegmentCount = result.SegmentCount;
        entity.StartShare = result.StartShare;
        entity.EndShare = result.EndShare;
        entity.Change = result.Change;
        entity.GrowthMultiple = result.GrowthMultiple;
        entity.WeeklyData = JsonHelper.Serialize(result.WeeklyData) ?? "[]";
        entity.UpdatedBy = userId;
        entity.UpdatedDate = now;
    }

    private static ComparisonSummaryDto MapToSummary(ComparisonSegment entity)
    {
        var sideA = entity.Results.FirstOrDefault(r => r.Side == ComparisonSide.A);
        var sideB = entity.Results.FirstOrDefault(r => r.Side == ComparisonSide.B);

        return new ComparisonSummaryDto
        {
            Id = entity.Id,
            Name = entity.Name,
            ProductGroup = entity.ProductGroup,
            WeekStart = entity.WeekStart,
            WeekEnd = entity.WeekEnd,
            CreatedDate = entity.CreatedDate,
            CreatedByName = entity.CreatedByUser != null
                ? $"{entity.CreatedByUser.FirstName} {entity.CreatedByUser.LastName}"
                : null,
            SegmentA = sideA != null ? MapToSideSummary(sideA) : null,
            SegmentB = sideB != null ? MapToSideSummary(sideB) : null
        };
    }

    private static ComparisonSideSummaryDto MapToSideSummary(ComparisonSegmentResult result)
    {
        return new ComparisonSideSummaryDto
        {
            Filters = MapFilters(result),
            EndShare = result.EndShare,
            Change = result.Change
        };
    }

    private static ComparisonDetailDto MapToDetail(ComparisonSegment entity)
    {
        var sideA = entity.Results.FirstOrDefault(r => r.Side == ComparisonSide.A);
        var sideB = entity.Results.FirstOrDefault(r => r.Side == ComparisonSide.B);

        return new ComparisonDetailDto
        {
            Id = entity.Id,
            Name = entity.Name,
            ProductGroup = entity.ProductGroup,
            WeekStart = entity.WeekStart,
            WeekEnd = entity.WeekEnd,
            CreatedDate = entity.CreatedDate,
            CreatedByName = entity.CreatedByUser != null
                ? $"{entity.CreatedByUser.FirstName} {entity.CreatedByUser.LastName}"
                : null,
            AiCommentDeepSeek = entity.AiCommentDeepSeek,
            AiCommentGemini = entity.AiCommentGemini,
            AiCommentGpt = entity.AiCommentGpt,
            SegmentA = sideA != null ? MapToSideDetail(sideA) : null,
            SegmentB = sideB != null ? MapToSideDetail(sideB) : null
        };
    }

    private static ComparisonSideDetailDto MapToSideDetail(ComparisonSegmentResult result)
    {
        return new ComparisonSideDetailDto
        {
            Filters = MapFilters(result),
            TotalPolicy = result.TotalPolicy,
            SegmentCount = result.SegmentCount,
            StartShare = result.StartShare,
            EndShare = result.EndShare,
            Change = result.Change,
            GrowthMultiple = result.GrowthMultiple,
            WeeklyData = JsonHelper.Deserialize<List<SegmentDriftWeekDto>>(result.WeeklyData) ?? new()
        };
    }

    private static SegmentFilterDto MapFilters(ComparisonSegmentResult result)
    {
        return new SegmentFilterDto
        {
            Brands = JsonHelper.DeserializeStringList(result.Brands),
            InsuredAges = JsonHelper.DeserializeStringList(result.InsuredAges),
            InsuredTypes = JsonHelper.DeserializeStringList(result.InsuredTypes),
            Genders = JsonHelper.DeserializeStringList(result.Genders),
            VehicleAges = JsonHelper.DeserializeStringList(result.VehicleAges),
            VehicleValues = JsonHelper.DeserializeStringList(result.VehicleValues)
        };
    }
}