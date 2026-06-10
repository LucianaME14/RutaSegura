using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RutaSegura.AI.Options;

namespace RutaSegura.AI.Services;

public class OllamaService : IOllamaService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly OllamaOptions _opts;
    private readonly ILogger<OllamaService> _logger;

    public OllamaService(
        IHttpClientFactory httpFactory,
        IOptions<OllamaOptions> opts,
        ILogger<OllamaService> logger)
    {
        _httpFactory = httpFactory;
        _opts = opts.Value;
        _logger = logger;
    }

    public string BaseUrl => _opts.BaseUrl.TrimEnd('/');
    public string CurrentModel => _opts.Model;

    public async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        if (!_opts.Enabled) return false;
        try
        {
            var client = CreateClient();
            using var resp = await client.GetAsync("/api/tags", ct);
            return resp.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Ollama no disponible en {Url}", BaseUrl);
            return false;
        }
    }

    public async Task<OllamaStatusDto> GetStatusAsync(CancellationToken ct = default)
    {
        if (!_opts.Enabled)
        {
            return new OllamaStatusDto(
                false,
                BaseUrl,
                CurrentModel,
                "Ollama deshabilitado en configuración (Ollama:Enabled=false).",
                null);
        }

        try
        {
            var client = CreateClient();
            using var resp = await client.GetAsync("/api/tags", ct);
            if (!resp.IsSuccessStatusCode)
            {
                return new OllamaStatusDto(
                    false,
                    BaseUrl,
                    CurrentModel,
                    $"Ollama no respondió ({(int)resp.StatusCode}). ¿Está corriendo `ollama serve`?",
                    null);
            }

            var json = await resp.Content.ReadFromJsonAsync<JsonElement>(ct);
            var models = new List<string>();
            if (json.TryGetProperty("models", out var arr))
            {
                foreach (var m in arr.EnumerateArray())
                {
                    if (m.TryGetProperty("name", out var name))
                        models.Add(name.GetString() ?? "");
                }
            }

            var tieneModelo = models.Any(m =>
                m.StartsWith(CurrentModel, StringComparison.OrdinalIgnoreCase)
                || m.Contains(CurrentModel, StringComparison.OrdinalIgnoreCase));

            return new OllamaStatusDto(
                true,
                BaseUrl,
                CurrentModel,
                tieneModelo
                    ? $"Ollama operativo. Modelo configurado: {CurrentModel}."
                    : $"Ollama activo pero no se encontró '{CurrentModel}'. Ejecuta: ollama pull {CurrentModel}",
                models);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al consultar Ollama");
            return new OllamaStatusDto(
                false,
                BaseUrl,
                CurrentModel,
                $"No se pudo conectar a Ollama en {BaseUrl}. Instala Ollama y ejecuta `ollama serve`.",
                null);
        }
    }

    private HttpClient CreateClient()
    {
        var client = _httpFactory.CreateClient("ollama");
        client.BaseAddress = new Uri(BaseUrl + "/");
        client.Timeout = TimeSpan.FromSeconds(_opts.TimeoutSeconds);
        return client;
    }
}
