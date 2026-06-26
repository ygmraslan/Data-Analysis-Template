namespace DataAnalysis.Application.Common.Settings;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpireMinutes { get; set; }
    public int RefreshTokenExpireDays { get; set; }
    public int MfaTokenExpireMinutes { get; set; }
    public int PasswordChangeTokenExpireMinutes { get; set; }
}