using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using RutaSegura.AI.Memory;
using RutaSegura.AI.Options;
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
    string Proveedor,
    bool DesdeCache,
    IReadOnlyList<string> Fuentes);

public interface ISafeBotAgentService
{
    Task<ChatAgentResponse> ProcessAsync(ChatAgentRequest request, CancellationToken ct = default);
}

/// <summary>
/// Agente SafeBot: Semantic Kernel orquesta plugins + Groq/Ollama genera respuesta natural.
/// </summary>
public class SafeBotAgentService : ISafeBotAgentService
{
    private readonly Kernel _kernel;
    private readonly ILlmStatusService _llm;
    private readonly SafeBotContextBuilder _contextBuilder;
    private readonly IChatCacheService _cache;
    private readonly ChatSessionMemory _memory;
    private readonly OllamaOptions _ollamaOpts;
    private readonly ILogger<SafeBotAgentService> _logger;

    public SafeBotAgentService(
        Kernel kernel,
        ILlmStatusService llm,
        SafeBotContextBuilder contextBuilder,
        IChatCacheService cache,
        ChatSessionMemory memory,
        IOptions<OllamaOptions> ollamaOpts,
        ILogger<SafeBotAgentService> logger)
    {
        _kernel = kernel;
        _llm = llm;
        _contextBuilder = contextBuilder;
        _cache = cache;
        _memory = memory;
        _ollamaOpts = ollamaOpts.Value;
        _logger = logger;
    }

    public async Task<ChatAgentResponse> ProcessAsync(
        ChatAgentRequest request,
        CancellationToken ct = default)
    {
        var message = (request.Message ?? "").Trim();
        if (message.Length == 0)
            return new ChatAgentResponse(
                "Escribe tu pregunta sobre rutas, zonas o reportes.",
                false, _llm.CurrentModel, _llm.Proveedor, false, []);

        var cached = await _cache.GetAsync(request.UserId, message, ct);
        if (!string.IsNullOrEmpty(cached))
        {
            return new ChatAgentResponse(
                cached, true, _llm.CurrentModel, _llm.Proveedor, true, ["Redis"]);
        }

        var (context, fuentes) = await _contextBuilder.BuildAsync(
            message,
            request.UserId,
            request.EsAdministrador,
            request.Zona,
            request.Origen,
            request.Destino,
            ct);

        string answer;
        var llmOk = await _llm.IsLlmAvailableAsync(ct);

        if (llmOk)
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
                _logger.LogWarning(ex, "Error al invocar LLM ({Proveedor}) vía Semantic Kernel", _llm.Proveedor);
                answer = SafeBotFallbackFormatter.Format(message, context);
            }
        }
        else
        {
            answer = SafeBotFallbackFormatter.Format(message, context);
            if (_ollamaOpts.Enabled && !_llm.UsaGroq)
                answer += " " + SafeBotPrompts.FallbackOllamaOff;
        }

        _memory.AddTurn(request.UserId, message, answer);
        await _cache.SetAsync(request.UserId, message, answer, ct);

        return new ChatAgentResponse(
            answer,
            llmOk,
            _llm.CurrentModel,
            _llm.Proveedor,
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

        var settings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.4,
            MaxTokens = 512,
        };

        var result = await chat.GetChatMessageContentAsync(history, settings, _kernel, ct);
        var content = result.Content?.Trim();
        return string.IsNullOrEmpty(content)
            ? SafeBotFallbackFormatter.Format(message, context)
            : content;
    }
}
