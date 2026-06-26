using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.Permissions.Queries.GetPermissions;

public class GetPermissionsQuery : IRequest<Result<GetPermissionsQueryResponse>>
{
    public int UserId { get; set; }
}