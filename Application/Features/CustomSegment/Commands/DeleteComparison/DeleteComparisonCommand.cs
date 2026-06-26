using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Commands.DeleteComparison;

public class DeleteComparisonCommand : IRequest<DeleteComparisonResponse>
{
    public int Id { get; set; }
    public int UserId { get; set; }
}