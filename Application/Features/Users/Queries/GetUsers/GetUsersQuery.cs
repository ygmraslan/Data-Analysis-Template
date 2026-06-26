using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.Users.Queries.GetUsers;

public class GetUsersQuery : IRequest<Result<GetUsersQueryResponse>>
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}