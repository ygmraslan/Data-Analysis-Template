using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.Auth.Queries.GetMe;

public class GetMeQuery : IRequest<Result<GetMeResponse>>
{
    public int UserId { get; set; }
}