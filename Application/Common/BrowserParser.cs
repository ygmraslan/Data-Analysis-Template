namespace DataAnalysis.Application.Common;

public static class BrowserParser
{
    public static string Parse(string userAgent)
    {
        if (userAgent.Contains("Edg"))     return "Edge";
        if (userAgent.Contains("Chrome"))  return "Chrome";
        if (userAgent.Contains("Firefox")) return "Firefox";
        if (userAgent.Contains("Safari"))  return "Safari";
        if (userAgent.Contains("Opera"))   return "Opera";
        return "Unknown";
    }
}