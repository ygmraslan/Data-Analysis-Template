namespace DataAnalysis.Domain.Entities.Permission;

public class Module
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}