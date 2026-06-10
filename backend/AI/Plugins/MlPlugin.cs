using System.ComponentModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using RutaSegura.Data;
using RutaSegura.ML;
using RutaSegura.Services;

namespace RutaSegura.AI.Plugins;

/// <summary>Plugin Semantic Kernel: clasificación ML.NET de zonas y recomendación de rutas.</summary>
public class MlPlugin
{
    private readonly MlNetService _ml;
    private readonly MlZoneQueryService _zoneQuery;
    private readonly ApplicationDbContext _db;

    public MlPlugin(MlNetService ml, MlZoneQueryService zoneQuery, ApplicationDbContext db)
    {
        _ml = ml;
        _zoneQuery = zoneQuery;
        _db = db;
    }

    [KernelFunction("clasificar_zona")]
    [Description("Clasifica una zona como Segura, Moderada o Peligrosa usando ML.NET.")]
    public async Task<string> ClasificarZonaAsync(
        [Description("Nombre de la zona")] string zona = "Lima")
    {
        await _ml.EnsureModelsAsync();
        var term = (zona ?? "Lima").Trim();
        var desde = DateTime.UtcNow.AddDays(-30);

        var reportesZona = await _db.Reportes
            .AsNoTracking()
            .CountAsync(r => r.Ubicacion.Contains(term) && r.FechaReporte >= desde);

        var hora = (float)DateTime.Now.Hour;
        var cantidadNorm = Math.Clamp(reportesZona / 20f, 0f, 1f);
        var incidentesRec = Math.Clamp(reportesZona / 10f, 0f, 1f);
        var iluminacion = hora >= 19 || hora < 6 ? 0.35f : 0.75f;
        var trafico = hora >= 7 && hora <= 20 ? 0.65f : 0.3f;

        var result = await _zoneQuery.ClasificarAsync(
            term,
            cantidadNorm,
            hora,
            iluminacion,
            trafico,
            incidentesRec);

        return JsonSerializer.Serialize(new
        {
            zona = term,
            clasificacion = result.Etiqueta,
            riesgo = result.Riesgo,
            confianza_pct = result.Confianza * 100,
            indicador = result.IndicadorVisual,
            motor = result.Motor,
            reportes_30d = reportesZona,
            mensaje_tecnico =
                $"La zona presenta riesgo {result.Riesgo.ToLowerInvariant()} según reportes y el modelo predictivo de seguridad (confianza {result.Confianza:P0}).",
        });
    }

    [KernelFunction("recomendar_ruta")]
    [Description("Recomienda perfiles de ruta (segura, rápida, equilibrada) con ML.NET.")]
    public async Task<string> RecomendarRutaAsync(
        int usuarioId,
        string origen = "Origen",
        string destino = "Destino")
    {
        await _ml.EnsureModelsAsync();
        var perfiles = _ml.RecommendRouteProfiles(usuarioId, origen, destino);
        var mejor = perfiles.FirstOrDefault();

        return JsonSerializer.Serialize(new
        {
            origen,
            destino,
            mejor_perfil = mejor?.VarianteId,
            mejor_nombre = mejor?.Nombre,
            seguridad_pct = mejor?.SeguridadPct,
            perfiles = perfiles.Select(p => new
            {
                p.VarianteId,
                p.Nombre,
                p.SeguridadPct,
                p.PreferenceScore,
            }),
            motor = "ML.NET Recommender",
        });
    }
}
