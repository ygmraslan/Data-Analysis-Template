namespace DataAnalysis.Domain.Entities.Permission;

public class Permission
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }


    public Module Module { get; set; } = null!;
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}