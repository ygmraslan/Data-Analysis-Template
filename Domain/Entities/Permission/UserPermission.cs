using DataAnalysis.Domain.Entities.Identity;

namespace DataAnalysis.Domain.Entities.Permission;

public class UserPermission
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PermissionId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedDate { get; set; }

    public User User { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}