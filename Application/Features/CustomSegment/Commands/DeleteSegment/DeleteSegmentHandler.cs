using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Commands.DeleteSegment;

public class DeleteSegmentHandler : IRequestHandler<DeleteSegmentCommand, DeleteSegmentResponse>
{
    private readonly ICustomSegmentDbRepository _repository;

    public DeleteSegmentHandler(ICustomSegmentDbRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeleteSegmentResponse> Handle(DeleteSegmentCommand request, CancellationToken cancellationToken)
    {
        var segment = await _repository.GetByIdAsync(request.SegmentId, cancellationToken);

        if (segment == null)
        {
            return new DeleteSegmentResponse
            {
                Success = false,
                Message = "Segment bulunamadı."
            };
        }

        await _repository.DeleteAsync(request.SegmentId, request.UserId, cancellationToken);

        return new DeleteSegmentResponse
        {
            Success = true,
            Message = "Segment başarıyla silindi."
        };
    }
}