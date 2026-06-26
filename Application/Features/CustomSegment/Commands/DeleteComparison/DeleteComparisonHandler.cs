using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Commands.DeleteComparison;

public class DeleteComparisonHandler : IRequestHandler<DeleteComparisonCommand, DeleteComparisonResponse>
{
    private readonly IComparisonRepository _repository;

    public DeleteComparisonHandler(IComparisonRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeleteComparisonResponse> Handle(DeleteComparisonCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Id, request.UserId, cancellationToken);

        return new DeleteComparisonResponse
        {
            Success = result,
            Message = result ? "Karşılaştırma silindi." : "Karşılaştırma bulunamadı."
        };
    }
}