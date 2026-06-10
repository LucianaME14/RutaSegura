using System.ComponentModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using RutaSegura.Data;
using RutaSegura.Services;

namespace RutaSegura.AI.Plugins;

/// <summary>Plugin Semantic Kernel: alertas del sistema y nivel de riesgo.</summary>
public class AlertasPlugin
{
    private readonly ApplicationDbContext _db;
    private readonly DashboardAlertasService _alertas;
    private readonly RedisService _redis;

    public AlertasPlugin(
        ApplicationDbContext db,
        DashboardAlertasService alertas,
        RedisService redis)
    {
        _db = db;
        _alertas = alertas;
        _redis = redis;
    }

    [KernelFunction("obtener_alertas_activas")]
    [Description("Lista alertas activas del sistema y reportes recientes de alto riesgo.")]
    public async Task<string> ObtenerAlertasActivasAsync(int max = 6)
    {
        var cacheKey = $"safebot:alertas:activas:{max}";
        if (_redis.IsEnabled)
        {
            var cached = await _redis.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached)) return cached;
        }

        var recientes = await _alertas.GetRecientesAsync(max, 14);
        var sistema = await _db.AlertasSistema
            .AsNoTracking()
            .OrderByDescending(a => a.CreadaEn)
            .Take(max)
            .Select(a => new
            {
                a.Titulo,
                a.Prioridad,
                a.UbicacionRef,
                a.RiesgoEstimadoPct,
            })
            .ToListAsync();

        var json = JsonSerializer.Serialize(new
        {
            alertas_sistema = sistema,
            reportes_destacados = recientes.Alertas.Take(max).Select(a => new
            {
                titulo = a.Titulo,
                ubicacion = a.Ubicacion,
                tipo = a.TipoIncidente,
                nivel = a.EtiquetaSeguridad,
                indicador = a.IndicadorVisual,
            }),
        });

        if (_redis.IsEnabled)
            await _redis.SetStringAsync(cacheKey, json, TimeSpan.FromMinutes(4));

        return json;
    }

    [KernelFunction("obtener_nivel_riesgo")]
    [Description("Estima el nivel de riesgo general o de una zona específica según reportes.")]
    public async Task<string> ObtenerNivelRiesgoAsync(string? zona = null)
    {
        var term = (zona ?? "").Trim();
        var cacheKey = $"safebot:riesgo:{term.ToLowerInvariant()}";
        if (_redis.IsEnabled)
        {
            var cached = await _redis.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached)) return cached;
        }

        var desde = DateTime.UtcNow.AddDays(-30);
        var query = _db.Reportes.AsNoTracking()
            .Where(r => r.FechaReporte >= desde && r.Estado != "Rechazado");

        if (!string.IsNullOrEmpty(term))
            query = query.Where(r => r.Ubicacion.Contains(term));

        var count = await query.CountAsync();
        var graves = await query.CountAsync(r =>
            r.TipoIncidente == "Robo" || r.TipoIncidente == "Asalto");

        var nivel = count switch
        {
            >= 15 => "Alto",
            >= 6 => "Moderado",
            _ => "Bajo",
        };
        if (graves >= 3) nivel = "Alto";

        var json = JsonSerializer.Serialize(new
        {
            zona = string.IsNullOrEmpty(term) ? "General" : term,
            nivel,
            reportes_30d = count,
            incidentes_graves = graves,
            recomendacion = nivel switch
            {
                "Alto" => "Evita la zona de noche; usa rutas iluminadas y comparte ubicación.",
                "Moderado" => "Precaución: revisa reportes recientes antes de transitar.",
                _ => "Zona relativamente tranquila según reportes recientes.",
            },
        });

        if (_redis.IsEnabled)
            await _redis.SetStringAsync(cacheKey, json, TimeSpan.FromMinutes(5));

        return json;
    }
}
