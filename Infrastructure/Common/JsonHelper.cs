using System.Text.Json;

namespace DataAnalysis.Infrastructure.Common;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static string? Serialize<T>(T? value)
    {
        if (value == null) return null;
        return JsonSerializer.Serialize(value, DefaultOptions);
    }

    public static List<string>? DeserializeStringList(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json, DefaultOptions);
        }
        catch
        {
            return null;
        }
    }

    public static T? Deserialize<T>(string? json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<T>(json, DefaultOptions);
        }
        catch
        {
            return null;
        }
    }
}