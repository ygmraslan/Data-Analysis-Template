namespace DataAnalysis.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    Task SendWelcomeEmailAsync(string toEmail, string fullName, string temporaryPassword, string loginLink);
    Task SendPasswordResetByAdminEmailAsync(string toEmail, string fullName, string newPassword, string loginLink);
}