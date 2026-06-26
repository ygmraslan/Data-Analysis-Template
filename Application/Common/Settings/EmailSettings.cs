namespace DataAnalysis.Application.Common.Settings;

public class EmailSettings
{
    public string Host { get; set; } = string.Empty;
    public string Exchange { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string FromName { get; set; } = string.Empty;
}