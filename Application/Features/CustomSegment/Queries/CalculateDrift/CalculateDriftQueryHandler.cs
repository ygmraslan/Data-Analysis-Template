using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using DataAnalysis.Application.Features.CustomSegment.Dtos;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.CalculateDrift;

public class CalculateDriftQueryHandler : IRequestHandler<CalculateDriftQuery, CalculateDriftQueryResponse>
{
    private readonly ICustomSegmentRepository _repository;

    public CalculateDriftQueryHandler(ICustomSegmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<CalculateDriftQueryResponse> Handle(CalculateDriftQuery request, CancellationToken cancellationToken)
    {
        var productGroup = Enum.Parse<ProductGroup>(request.ProductGroup);
        
        var filters = new SegmentFilterDto
        {
            Brands = request.Filters.Brands,
            InsuredAges = request.Filters.InsuredAges,
            InsuredTypes = request.Filters.InsuredTypes,
            Genders = request.Filters.Genders,
            VehicleAges = request.Filters.VehicleAges,
            VehicleValues = request.Filters.VehicleValues
        };

        var startDate = request.WeekStart.AddDays(-49);
        var endDate = request.WeekEnd;

        var weeklyData = await _repository.CalculateDriftAsync(
            productGroup,
            filters,
            startDate,
            endDate,
            cancellationToken);

        if (weeklyData.Count == 0)
            return new CalculateDriftQueryResponse();

        var firstWeek = weeklyData.First();
        var lastWeek = weeklyData.Last();

        var weekItems = new List<CalculateDriftWeekItem>();
        for (int i = 0; i < weeklyData.Count; i++)
        {
            var week = weeklyData[i];
            decimal? wow = null;
            
            if (i > 0)
            {
                var prevShare = weeklyData[i - 1].SegmentShare;
                if (prevShare > 0)
                {
                    wow = Math.Round((week.SegmentShare - prevShare) / prevShare * 100, 2);
                }
            }

            weekItems.Add(new CalculateDriftWeekItem
            {
                WeekStart = week.WeekStart,
                WeekLabel = week.WeekLabel,
                TotalPolicy = week.TotalPolicy,
                SegmentCount = week.SegmentCount,
                SegmentShare = week.SegmentShare,
                WoW = wow
            });
        }

        return new CalculateDriftQueryResponse
        {
            TotalPolicy = weeklyData.Sum(x => x.TotalPolicy),
            SegmentCount = weeklyData.Sum(x => x.SegmentCount),
            StartShare = firstWeek.SegmentShare,
            EndShare = lastWeek.SegmentShare,
            Change = lastWeek.SegmentShare - firstWeek.SegmentShare,
            GrowthMultiple = firstWeek.SegmentShare > 0
                ? Math.Round(lastWeek.SegmentShare / firstWeek.SegmentShare, 2)
                : 0,
            WeeklyData = weekItems
        };
    }
}