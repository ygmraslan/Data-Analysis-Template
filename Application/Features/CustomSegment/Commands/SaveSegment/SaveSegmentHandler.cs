using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using DataAnalysis.Application.Features.CustomSegment.Dtos;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Commands.SaveSegment;

public class SaveSegmentHandler : IRequestHandler<SaveSegmentCommand, SaveSegmentResponse>
{
    private readonly ICustomSegmentDbRepository _dbRepository;

    public SaveSegmentHandler(ICustomSegmentDbRepository dbRepository)
    {
        _dbRepository = dbRepository;
    }

    public async Task<SaveSegmentResponse> Handle(SaveSegmentCommand request, CancellationToken cancellationToken)
    {
        var filters = new SegmentFilterDto
        {
            Brands = request.Filters.Brands,
            InsuredAges = request.Filters.InsuredAges,
            InsuredTypes = request.Filters.InsuredTypes,
            Genders = request.Filters.Genders,
            VehicleAges = request.Filters.VehicleAges,
            VehicleValues = request.Filters.VehicleValues
        };

        var segmentDto = new SegmentDto
        {
            Name = request.Name,
            ProductGroup = request.ProductGroup,
            Filters = filters
        };

        var savedSegment = await _dbRepository.CreateAsync(segmentDto, request.UserId, cancellationToken);

        // Drift'i yeniden hesaplamıyoruz — ekranda kullanıcının gördüğü sonucu olduğu gibi kaydediyoruz.
        if (request.Result.WeeklyData.Count == 0)
        {
            return new SaveSegmentResponse
            {
                SegmentId = savedSegment.Id,
                Name = savedSegment.Name,
                Success = false,
                Message = "Kaydedilecek hesaplama sonucu bulunamadı."
            };
        }

        var startDate = request.WeekStart.AddDays(-49); // 8 hafta geriye
        var endDate = request.WeekEnd;

        var resultDto = new SegmentResultDto
        {
            SegmentId = savedSegment.Id,
            StartDate = startDate,
            EndDate = endDate,
            TotalPolicy = request.Result.TotalPolicy,
            SegmentCount = request.Result.SegmentCount,
            StartShare = request.Result.StartShare,
            EndShare = request.Result.EndShare,
            Change = request.Result.Change,
            GrowthMultiple = request.Result.GrowthMultiple,
            WeeklyData = request.Result.WeeklyData,
            AiCommentDeepSeek = request.AiComments?.DeepSeek,
            AiCommentGemini = request.AiComments?.Gemini,
            AiCommentGpt = request.AiComments?.Gpt
        };

        var savedResult = await _dbRepository.AddResultAsync(savedSegment.Id, resultDto, request.UserId, cancellationToken);

        return new SaveSegmentResponse
        {
            SegmentId = savedSegment.Id,
            ResultId = savedResult.Id,
            Name = savedSegment.Name,
            Success = true,
            Message = "Segment başarıyla kaydedildi."
        };
    }
}