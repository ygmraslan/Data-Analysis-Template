using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.Permissions.Commands.AssignPermissions;

public class AssignPermissionsCommand : IRequest<Result>
{
    public int UserId { get; set; }
    public List<int> PermissionIds { get; set; } = new();
}