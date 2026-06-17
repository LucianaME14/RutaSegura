using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RutaSegura.AI.Services;

namespace RutaSegura.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly ISafeBotAgentService _agent;
    private readonly ILlmStatusService _llm;

    public ChatController(ISafeBotAgentService agent, ILlmStatusService llm)
    {
        _agent = agent;
        _llm = llm;
    }

    /// <summary>SafeBot — Semantic Kernel + Groq/Ollama + plugins ML.NET.</summary>
    [HttpPost]
    public async Task<ActionResult<ChatApiResponse>> Post(
        [FromBody] ChatApiRequest body,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.Message))
            return BadRequest(new { message = "El campo message es requerido." });

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var esAdmin = User.IsInRole("Administrador");

        var result = await _agent.ProcessAsync(
            new ChatAgentRequest(
                body.Message.Trim(),
                userId,
                esAdmin,
                body.Zona,
                body.Origen,
                body.Destino),
            ct);

        return Ok(new ChatApiResponse
        {
            Answer = result.Answer,
            LlmActivo = result.LlmActivo,
            Modelo = result.Modelo,
            Proveedor = result.Proveedor,
            DesdeCache = result.DesdeCache,
            Fuentes = result.Fuentes,
        });
    }

    /// <summary>Estado del LLM (Groq u Ollama).</summary>
    [HttpGet("status")]
    public async Task<ActionResult<LlmStatusDto>> Status(CancellationToken ct)
    {
        return Ok(await _llm.GetStatusAsync(ct));
    }
}

public class ChatApiRequest
{
    public string Message { get; set; } = "";
    public string? Zona { get; set; }
    public string? Origen { get; set; }
    public string? Destino { get; set; }
}

public class ChatApiResponse
{
    public string Answer { get; set; } = "";
    public bool LlmActivo { get; set; }
    public string Modelo { get; set; } = "";
    public string Proveedor { get; set; } = "";
    public bool DesdeCache { get; set; }
    public IReadOnlyList<string>? Fuentes { get; set; }
}
