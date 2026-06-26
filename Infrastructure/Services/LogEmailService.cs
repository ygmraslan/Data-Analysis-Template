using DataAnalysis.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Infrastructure.Services;

public class LogEmailService : IEmailService
{
    private readonly ILogger<LogEmailService> _logger;

    public LogEmailService(ILogger<LogEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        _logger.LogWarning(
            "[DEV-MAIL] Şifre sıfırlama maili | To: {Email} | Link: {Link}",
            toEmail, resetLink);
        return Task.CompletedTask;
    }

    public Task SendWelcomeEmailAsync(string toEmail, string fullName, string temporaryPassword, string loginLink)
    {
        _logger.LogWarning(
            "[DEV-MAIL] Hoş geldin maili | To: {Email} | Ad: {Name} | Geçici Şifre: {Password} | Link: {Link}",
            toEmail, fullName, temporaryPassword, loginLink);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetByAdminEmailAsync(string toEmail, string fullName, string newPassword, string loginLink)
    {
        _logger.LogWarning(
            "[DEV-MAIL] Admin şifre sıfırlama maili | To: {Email} | Ad: {Name} | Yeni Şifre: {Password} | Link: {Link}",
            toEmail, fullName, newPassword, loginLink);
        return Task.CompletedTask;
    }
}