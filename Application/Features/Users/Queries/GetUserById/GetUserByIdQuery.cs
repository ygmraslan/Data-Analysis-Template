using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<Result<GetUserByIdQueryResponse>>
{
    public int Id { get; set; }
}