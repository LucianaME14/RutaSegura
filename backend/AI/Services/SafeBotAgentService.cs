using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using RutaSegura.AI.Memory;
using RutaSegura.AI.Prompts;

namespace RutaSegura.AI.Services;

public record ChatAgentRequest(
    string Message,
    int UserId,
    bool EsAdministrador,
    string? Zona = null,
    string? Origen = null,
    string? Destino = null);

public record ChatAgentResponse(
    string Answer,
    bool LlmActivo,
    string Modelo,
    bool DesdeCache,
    IReadOnlyList<string> Fuentes);

public interface ISafeBotAgentService
{
    Task<ChatAgentResponse> ProcessAsync(ChatAgentRequest request, CancellationToken ct = default);
}

/// <summary>
/// Agente SafeBot: Semantic Kernel orquesta plugins + Ollama genera respuesta natural.
/// </summary>
public class SafeBotAgentService : ISafeBotAgentService
{
    private readonly Kernel _kernel;
    private readonly IOllamaService _ollama;
    private readonly SafeBotContextBuilder _contextBuilder;
    private readonly IChatCacheService _cache;
    private readonly ChatSessionMemory _memory;
    private readonly ILogger<SafeBotAgentService> _logger;

    public SafeBotAgentService(
        Kernel kernel,
        IOllamaService ollama,
        SafeBotContextBuilder contextBuilder,
        IChatCacheService cache,
        ChatSessionMemory memory,
        ILogger<SafeBotAgentService> logger)
    {
        _kernel = kernel;
        _ollama = ollama;
        _contextBuilder = contextBuilder;
        _cache = cache;
        _memory = memory;
        _logger = logger;
    }

    public async Task<ChatAgentResponse> ProcessAsync(
        ChatAgentRequest request,
        CancellationToken ct = default)
    {
        var message = (request.Message ?? "").Trim();
        if (message.Length == 0)
            return new ChatAgentResponse("Escribe tu pregunta sobre rutas, zonas o reportes.", false, _ollama.CurrentModel, false, []);

        var cached = await _cache.GetAsync(request.UserId, message, ct);
        if (!string.IsNullOrEmpty(cached))
        {
            return new ChatAgentResponse(cached, true, _ollama.CurrentModel, true, ["Redis"]);
        }

        var (context, fuentes) = await _contextBuilder.BuildAsync(
            message,
            request.UserId,
            request.EsAdministrador,
            request.Zona,
            request.Origen,
            request.Destino,
            ct);

        var lower = message.ToLowerInvariant();
        string answer;

        var ollamaOk = await _ollama.IsAvailableAsync(ct);
        if (ollamaOk)
        {
            try
            {
                answer = await GenerateWithSemanticKernelAsync(
                    message,
                    context,
                    request.EsAdministrador,
                    request.UserId,
                    ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al invocar Ollama vía Semantic Kernel");
                answer = BuildFallbackAnswer(message, context, lower);
            }
        }
        else
        {
            answer = BuildFallbackAnswer(message, context, lower);
            if (!answer.Contains("modo datos", StringComparison.OrdinalIgnoreCase))
                answer += " " + SafeBotPrompts.FallbackOllamaOff;
        }

        _memory.AddTurn(request.UserId, message, answer);
        await _cache.SetAsync(request.UserId, message, answer, ct);

        return new ChatAgentResponse(
            answer,
            ollamaOk,
            _ollama.CurrentModel,
            false,
            fuentes);
    }

    private async Task<string> GenerateWithSemanticKernelAsync(
        string message,
        string context,
        bool esAdmin,
        int userId,
        CancellationToken ct)
    {
        var chat = _kernel.GetRequiredService<IChatCompletionService>();
        var history = new ChatHistory();
        history.AddSystemMessage(SafeBotPrompts.System(esAdmin));

        foreach (var turn in _memory.GetHistory(userId).TakeLast(4))
        {
            history.AddUserMessage(turn.User);
            history.AddAssistantMessage(turn.Bot);
        }

        history.AddUserMessage(
            SafeBotPrompts.UserTemplate
                .Replace("{{$message}}", message)
                .Replace("{{$context}}", context));

#pragma warning disable SKEXP0010
        var settings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.4,
            MaxTokens = 512,
        };
#pragma warning restore SKEXP0010

        var result = await chat.GetChatMessageContentAsync(history, settings, _kernel, ct);
        var content = result.Content?.Trim();
        return string.IsNullOrEmpty(content)
            ? BuildFallbackAnswer(message, context, message.ToLowerInvariant())
            : content;
    }

    private static string BuildFallbackAnswer(string message, string context, string lower)
    {
        if (lower.Contains("sos") || lower.Contains("emergencia") || lower.Contains("robaron"))
            return SafeBotPrompts.FallbackSos;

        if (lower.Contains("reportar") || lower.Contains("hueco"))
            return SafeBotPrompts.FallbackReportar;

        if (context.Contains("mensaje_tecnico", StringComparison.OrdinalIgnoreCase))
        {
            var idx = context.IndexOf("mensaje_tecnico", StringComparison.OrdinalIgnoreCase);
            var slice = context[idx..Math.Min(context.Length, idx + 200)];
            var start = slice.IndexOf(':');
            if (start > 0)
            {
                var end = slice.IndexOf('"', start + 2);
                if (end > start)
                    return slice[(start + 2)..end];
            }
        }

        if (context.Contains("clasificacion", StringComparison.OrdinalIgnoreCase))
        {
            return "Según ML.NET y los reportes en la base de datos, revisa el nivel de riesgo en el contexto. "
                   + "Usa el mapa para ver incidentes cercanos y el botón SOS si hay emergencia.";
        }

        return "He consultado los datos de Ruta Segura. "
               + "Puedes preguntarme por zonas seguras, rutas recomendadas, reportes cercanos, clima o el botón SOS. "
               + $"Resumen técnico: {Truncate(context, 400)}";
    }

    private static string Truncate(string s, int max) =>
        s.Length <= max ? s : s[..max] + "…";
}
