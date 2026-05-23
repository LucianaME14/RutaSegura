using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RutaSegura.Services;

namespace RutaSegura.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MlController : ControllerBase
{
    private readonly MlNetService _ml;
    private readonly MlZoneQueryService _zonas;
    private readonly ExternalApisService _external;

    public MlController(MlNetService ml, MlZoneQueryService zonas, ExternalApisService external)
    {
        _ml = ml;
        _zonas = zonas;
        _external = external;
    }

    /// <summary>Estado de los modelos ML.NET (clasificación y recomendación).</summary>
    [HttpGet("estado")]
    [AllowAnonymous]
    public async Task<IActionResult> GetEstado(CancellationToken ct) =>
        Ok(await _ml.GetStatusAsync(ct));

    /// <summary>Clasifica el tipo de incidente a partir de contexto del reporte.</summary>
    [HttpPost("clasificar-incidente")]
    [AllowAnonymous]
    public async Task<IActionResult> ClasificarIncidente(
        [FromBody] ClasificarIncidenteRequest req,
        CancellationToken ct)
    {
        await _ml.EnsureModelsAsync(ct);
        var result = _ml.ClassifyIncident(
            req.Descripcion,
            req.Ubicacion,
            req.HasCoordinates,
            req.FechaReporte);

        if (result == null)
            return StatusCode(503, new { message = "Modelo de clasificación no disponible." });

        return Ok(result);
    }

    /// <summary>Recomienda variantes de ruta (segura / rápida / equilibrada) con Matrix Factorization.</summary>
    [HttpGet("recomendar-rutas")]
    [Authorize]
    public async Task<IActionResult> RecomendarRutas(
        [FromQuery] string origen,
        [FromQuery] string destino,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(origen) || string.IsNullOrWhiteSpace(destino))
            return BadRequest(new { message = "origen y destino son obligatorios." });

        await _ml.EnsureModelsAsync(ct);
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var items = _ml.RecommendRouteProfiles(userId, origen.Trim(), destino.Trim());

        return Ok(
            new
            {
                origen,
                destino,
                motor = "ML.NET MatrixFactorization",
                recomendaciones = items,
            });
    }

    /// <summary>Reentrena ambos modelos con datos de la base (admin / desarrollo).</summary>
    [HttpPost("entrenar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Entrenar(CancellationToken ct)
    {
        var result = await _ml.RetrainAsync(ct);
        return Ok(
            new
            {
                message = "Modelos ML.NET entrenados y guardados.",
                clasificacion = result.Incident,
                recomendacion = result.Recommendation,
                seguridadZona = result.ZoneSafety,
            });
    }

    /// <summary>
    /// Clasifica una zona (ML.NET Model Builder — Data Classification).
    /// Ejemplo: GET /api/ml/clasificar-zona?zona=Centro%20de%20Lima&amp;cantidad_reportes=0.7&amp;hora=22&amp;iluminacion=0.2&amp;trafico=0.8&amp;incidentes_recientes=4
    /// </summary>
    [HttpGet("clasificar-zona")]
    [AllowAnonymous]
    public async Task<IActionResult> ClasificarZonaGet(
        [FromQuery] string zona = "Lima",
        [FromQuery] float cantidad_reportes = 0.3f,
        [FromQuery] float hora = 12f,
        [FromQuery] float iluminacion = 0.6f,
        [FromQuery] float trafico = 0.4f,
        [FromQuery] float incidentes_recientes = 0f,
        CancellationToken ct = default)
    {
        var response = await _zonas.ClasificarAsync(
            zona,
            cantidad_reportes,
            hora,
            iluminacion,
            trafico,
            incidentes_recientes,
            ct);

        return Ok(new
        {
            response.Zona,
            riesgo = response.Riesgo,
            confianza = response.Confianza,
            response.IndicadorVisual,
            etiqueta = response.Etiqueta,
            response.Motor,
            response.ServidoDesdeCache,
        });
    }

    /// <summary>Clasifica seguridad de zona (POST JSON).</summary>
    [HttpPost("clasificar-zona")]
    [AllowAnonymous]
    public async Task<IActionResult> ClasificarZonaPost(
        [FromBody] ClasificarZonaRequest req,
        CancellationToken ct)
    {
        var response = await _zonas.ClasificarAsync(
            req.Zona ?? "Lima",
            req.CantidadReportes,
            req.Hora,
            req.Iluminacion,
            req.Trafico,
            req.IncidentesRecientes,
            ct);
        return Ok(response);
    }

    /// <summary>Contexto externo + ML para una zona (clima demo + clasificación).</summary>
    [HttpGet("contexto-zona")]
    [AllowAnonymous]
    public async Task<IActionResult> ContextoZona(
        [FromQuery] string zona = "Centro de Lima",
        [FromQuery] double lat = -12.0464,
        [FromQuery] double lon = -77.0428,
        CancellationToken ct = default)
    {
        var clima = await _external.GetClimaAsync(lat, lon, ct);
        var trafico = _external.GetTraficoDemo(zona);
        var ml = await _zonas.ClasificarAsync(
            zona,
            0.5f,
            (float)DateTime.Now.Hour,
            clima.TemperaturaC < 18 ? 0.4f : 0.7f,
            trafico.FactorNormalizado,
            1f,
            ct);

        return Ok(new { zona, clima, trafico, clasificacion = ml });
    }

    public class ClasificarZonaRequest
    {
        public string? Zona { get; set; }
        public float CantidadReportes { get; set; }
        public float Trafico { get; set; }
        public float Iluminacion { get; set; }
        public float Hora { get; set; } = 12f;
        public float IncidentesRecientes { get; set; }
    }

    public class ClasificarIncidenteRequest
    {
        public string? Descripcion { get; set; }
        public string? Ubicacion { get; set; }
        public bool HasCoordinates { get; set; }
        public DateTime? FechaReporte { get; set; }
    }
}
