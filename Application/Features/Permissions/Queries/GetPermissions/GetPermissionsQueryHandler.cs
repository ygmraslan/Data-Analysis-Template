using DataAnalysis.Application.Common;
using DataAnalysis.Application.Features.Permissions.Abstractions;
using DataAnalysis.Application.Features.Permissions.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.Permissions.Queries.GetPermissions;

public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, Result<GetPermissionsQueryResponse>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly ILogger<GetPermissionsQueryHandler> _logger;

    public GetPermissionsQueryHandler(
        IPermissionRepository permissionRepository,
        ILogger<GetPermissionsQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _logger = logger;
    }

    public async Task<Result<GetPermissionsQueryResponse>> Handle(
        GetPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching permissions for UserId: {UserId}", request.UserId);

        var modules = await _permissionRepository.GetModulesWithPermissionsAsync(cancellationToken);
        var userPermissionIds = await _permissionRepository.GetUserPermissionIdsAsync(request.UserId, cancellationToken);

        var response = new GetPermissionsQueryResponse
        {
            Modules = modules.Select(m => new ModuleDto
            {
                Id = m.Id,
                Name = m.Name,
                Permissions = m.Permissions.Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description ?? string.Empty,
                    Route = p.Route,
                    IsAssigned = userPermissionIds.Contains(p.Id)
                }).ToList()
            }).ToList()
        };

        return Result<GetPermissionsQueryResponse>.Ok(response);
    }
}