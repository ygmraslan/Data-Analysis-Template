using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Common.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace DataAnalysis.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    private SmtpClient CreateSmtpClient()
    {
        var client = new SmtpClient(_emailSettings.Host)
        {
            Port = _emailSettings.Port,
            EnableSsl = _emailSettings.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
        };

        if (!string.IsNullOrEmpty(_emailSettings.UserName) && !string.IsNullOrEmpty(_emailSettings.Password))
        {
            client.Credentials = new NetworkCredential(
                _emailSettings.UserName,
                _emailSettings.Password,
                _emailSettings.Domain
            );
        }

        return client;
    }

    private MailMessage CreateMailMessage(string toEmail, string subject, string body)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.FromAddress, _emailSettings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(toEmail);
        return mailMessage;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        var subject = "DataAnalysis - Şifre Sıfırlama Talebi";
        var body = $@"
            <html>
            <body>
                <h2>Şifre Sıfırlama Talebi</h2>
                <p>Şifrenizi sıfırlamak için bir talep aldık.</p>
                <p>Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın. Bu bağlantı 1 saat içinde geçerliliğini yitirecektir.</p>
                <a href='{resetLink}'>Şifremi Sıfırla</a>
                <p>Eğer böyle bir talepte bulunmadıysanız bu maili dikkate almayınız.</p>
            </body>
            </html>";

        using var smtpClient = CreateSmtpClient();
        using var mailMessage = CreateMailMessage(toEmail, subject, body);
        await smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string fullName, string temporaryPassword, string loginLink)
    {
        var subject = "DataAnalysis - Hesabınız Oluşturuldu";
        var body = $@"
            <html>
            <body>
                <h2>Hoş Geldiniz, {fullName}!</h2>
                <p>DataAnalysis sistemine hesabınız oluşturulmuştur.</p>
                <p>Aşağıdaki bilgilerle sisteme giriş yapabilirsiniz:</p>
                <ul>
                    <li><strong>E-posta:</strong> {toEmail}</li>
                    <li><strong>Geçici Şifre:</strong> {temporaryPassword}</li>
                </ul>
                <p>Güvenliğiniz için ilk girişinizde şifrenizi değiştirmeniz gerekmektedir.</p>
                <a href='{loginLink}'>Sisteme Giriş Yap</a>
                <p>Bu maili siz talep etmediyseniz lütfen sistem yöneticinizle iletişime geçin.</p>
            </body>
            </html>";

        using var smtpClient = CreateSmtpClient();
        using var mailMessage = CreateMailMessage(toEmail, subject, body);
        await smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendPasswordResetByAdminEmailAsync(string toEmail, string fullName, string newPassword, string loginLink)
    {
        var subject = "DataAnalysis - Şifreniz Sıfırlandı";
        var body = $@"
            <html>
            <body>
                <h2>Sayın {fullName},</h2>
                <p>Sistem yöneticisi tarafından DataAnalysis hesabınızın şifresi sıfırlanmıştır.</p>
                <p>Aşağıdaki bilgilerle sisteme giriş yapabilirsiniz:</p>
                <ul>
                    <li><strong>E-posta:</strong> {toEmail}</li>
                    <li><strong>Yeni Geçici Şifre:</strong> {newPassword}</li>
                </ul>
                <p>Güvenliğiniz için ilk girişinizde şifrenizi değiştirmeniz gerekmektedir.</p>
                <a href='{loginLink}'>Sisteme Giriş Yap</a>
                <p><strong>Bu işlemi siz talep etmediyseniz lütfen sistem yöneticinizle derhal iletişime geçin.</strong></p>
            </body>
            </html>";

        using var smtpClient = CreateSmtpClient();
        using var mailMessage = CreateMailMessage(toEmail, subject, body);
        await smtpClient.SendMailAsync(mailMessage);
    }
}