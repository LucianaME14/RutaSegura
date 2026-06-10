namespace RutaSegura.AI.Options;

/// <summary>Configuración de Ollama (LLM local). Cambiar modelo en appsettings o .env.</summary>
public class OllamaOptions
{
    public const string SectionName = "Ollama";

    public string BaseUrl { get; set; } = "http://localhost:11434";

    /// <summary>llama3 | mistral | deepseek-r1 | phi3</summary>
    public string Model { get; set; } = "llama3";

    public bool Enabled { get; set; } = true;

    public int TimeoutSeconds { get; set; } = 120;

    public static readonly string[] ModelosSoportados =
        ["llama3", "mistral", "deepseek-r1", "phi3", "llama3.2", "gemma2"];
}
