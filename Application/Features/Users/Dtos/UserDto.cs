namespace DataAnalysis.Application.Features.Users.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool HasMfa { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime CreatedDate { get; set; }
}