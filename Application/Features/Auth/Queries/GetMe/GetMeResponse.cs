namespace DataAnalysis.Application.Features.Auth.Queries.GetMe;

public class GetMeResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public List<string> Permissions { get; set; } = new();
}