namespace DataAnalysis.Application.Features.Permissions.Dtos;

public class PermissionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public bool IsAssigned { get; set; }
}