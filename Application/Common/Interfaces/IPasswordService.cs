namespace DataAnalysis.Application.Common.Interfaces;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
    bool ValidatePasswordStrength(string password);
    string GenerateTemporaryPassword();
}