namespace DataAnalysis.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(int userId, string email);
    string GenerateRefreshToken();
    string GenerateMfaToken();
    string HashToken(string token);
    string GeneratePasswordChangeToken(int userId);
    int? ValidatePasswordChangeToken(string token);
}