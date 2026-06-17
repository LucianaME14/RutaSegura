namespace RutaSegura.AI.Options;

/// <summary>LLM en la nube vía Groq (API compatible OpenAI). Ideal para Render.</summary>
public class GroqOptions
{
    public const string SectionName = "Groq";

    public string ApiKey { get; set; } = "";

    public string Model { get; set; } = "llama-3.1-8b-instant";

    public string BaseUrl { get; set; } = "https://api.groq.com/openai/v1";

    public bool Enabled { get; set; } = true;
}
