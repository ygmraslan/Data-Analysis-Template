namespace DataAnalysis.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool HasMfa { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}