namespace DataAnalysis.Application.Features.Permissions.Dtos;

public class ModuleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<PermissionDto> Permissions { get; set; } = new();
}