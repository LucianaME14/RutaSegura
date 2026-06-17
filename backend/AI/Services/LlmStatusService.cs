using Microsoft.Extensions.Options;
using RutaSegura.AI.Options;

namespace RutaSegura.AI.Services;

/// <summary>Estado del LLM activo: Groq (nube) tiene prioridad sobre Ollama (local).</summary>
public class LlmStatusService : ILlmStatusService
{
    private readonly GroqOptions _groq;
    private readonly OllamaOptions _ollamaOpts;
    private readonly IOllamaService _ollama;

    public LlmStatusService(
        IOptions<GroqOptions> groq,
        IOptions<OllamaOptions> ollamaOpts,
        IOllamaService ollama)
    {
        _groq = groq.Value;
        _ollamaOpts = ollamaOpts.Value;
        _ollama = ollama;
    }

    public bool UsaGroq => _groq.Enabled && !string.IsNullOrWhiteSpace(_groq.ApiKey);

    public string Proveedor => UsaGroq ? "Groq" : "Ollama";

    public string CurrentModel => UsaGroq ? _groq.Model : _ollama.CurrentModel;

    public async Task<bool> IsLlmAvailableAsync(CancellationToken ct = default)
    {
        if (UsaGroq) return true;
        if (!_ollamaOpts.Enabled) return false;
        return await _ollama.IsAvailableAsync(ct);
    }

    public async Task<LlmStatusDto> GetStatusAsync(CancellationToken ct = default)
    {
        if (UsaGroq)
        {
            return new LlmStatusDto(
                true,
                "Groq",
                _groq.Model,
                $"Groq operativo. Modelo: {_groq.Model} (LLM en la nube).");
        }

        var ollama = await _ollama.GetStatusAsync(ct);
        return new LlmStatusDto(
            ollama.Disponible,
            "Ollama",
            ollama.Modelo,
            ollama.Mensaje,
            ollama.ModelosInstalados);
    }
}
