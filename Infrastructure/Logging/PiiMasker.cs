using System.Text.RegularExpressions;

namespace DataAnalysis.Infrastructure.Logging;
public static class PiiMasker
{
    private static readonly Regex EmailRegex = new(
        @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        TimeSpan.FromSeconds(1));

    public static string MaskEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "***";

        var atIndex = email.IndexOf('@');
        if (atIndex <= 0)
            return "***";

        var localPart = email[..atIndex];
        var domainPart = email[(atIndex + 1)..];

        var maskedLocal = MaskLocalPart(localPart);
        var maskedDomain = MaskDomain(domainPart);

        return $"{maskedLocal}@{maskedDomain}";
    }
    public static string MaskAllEmails(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text ?? string.Empty;

        return EmailRegex.Replace(text, match => MaskEmail(match.Value));
    }

    private static string MaskLocalPart(string localPart)
    {
        if (string.IsNullOrEmpty(localPart))
            return "***";

        var parts = localPart.Split('.');
        var maskedParts = new List<string>();

        foreach (var part in parts)
        {
            if (string.IsNullOrEmpty(part))
            {
                maskedParts.Add("***");
                continue;
            }
            var visibleLength = Math.Min(2, part.Length);
            var visible = part[..visibleLength];
            maskedParts.Add($"{visible}***");
        }

        return string.Join(".", maskedParts);
    }

    private static string MaskDomain(string domain)
    {
        if (string.IsNullOrEmpty(domain))
            return "***";

        var parts = domain.Split('.');
        
        if (parts.Length == 1)
            return "***";
        if (parts.Length == 2)
        {
            return $"***.{parts[1]}";
        }
        else
        {
            var tld = string.Join(".", parts.Skip(parts.Length - 2));
            return $"***.{tld}";
        }
    }
}