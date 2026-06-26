namespace DataAnalysis.Application.Common.Settings;

public class OllamaSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 180;
    public ModelsConfig Models { get; set; } = new();
    public string Model { get; set; } = string.Empty;
}

public class ModelsConfig
{
    public ModelConfig DeepSeek { get; set; } = new();
    public ModelConfig Gemini { get; set; } = new();
    public ModelConfig Gpt { get; set; } = new();
}

public class ModelConfig
{
    public string Name { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
}