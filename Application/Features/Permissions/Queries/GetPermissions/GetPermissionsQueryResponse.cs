using DataAnalysis.Application.Features.Permissions.Dtos;

namespace DataAnalysis.Application.Features.Permissions.Queries.GetPermissions;

public class GetPermissionsQueryResponse
{
    public List<ModuleDto> Modules { get; set; } = new();
}