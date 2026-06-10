using System.ComponentModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using RutaSegura.Data;
using RutaSegura.Services;

namespace RutaSegura.AI.Plugins;

/// <summary>Plugin Semantic Kernel: consultas de reportes (SQLite + Redis).</summary>
public class ReportesPlugin
{
    private readonly ApplicationDbContext _db;
    private readonly RedisService _redis;

    public ReportesPlugin(ApplicationDbContext db, RedisService redis)
    {
        _db = db;
        _redis = redis;
    }

    [KernelFunction("obtener_reportes_recientes")]
    [Description("Obtiene los reportes de incidentes más recientes aprobados o pendientes.")]
    public async Task<string> ObtenerReportesRecientesAsync(
        [Description("Cantidad máxima de reportes")] int max = 5)
    {
        var n = Math.Clamp(max, 1, 15);
        var cacheKey = $"safebot:reportes:recientes:{n}";
        if (_redis.IsEnabled)
        {
            var cached = await _redis.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached)) return cached;
        }

        var desde = DateTime.UtcNow.AddDays(-30);
        var items = await _db.Reportes
            .AsNoTracking()
            .Where(r => r.FechaReporte >= desde && r.Estado != "Rechazado")
            .OrderByDescending(r => r.FechaReporte)
            .Take(n)
            .Select(r => new
            {
                r.TipoIncidente,
                r.Ubicacion,
                r.Estado,
                r.FechaReporte,
            })
            .ToListAsync();

        var json = JsonSerializer.Serialize(new
        {
            total = items.Count,
            reportes = items.Select(r => new
            {
                tipo = r.TipoIncidente,
                ubicacion = r.Ubicacion,
                estado = r.Estado,
                hace = $"{(int)(DateTime.UtcNow - r.FechaReporte).TotalHours}h",
            }),
        });

        if (_redis.IsEnabled)
            await _redis.SetStringAsync(cacheKey, json, TimeSpan.FromMinutes(5));

        return json;
    }

    [KernelFunction("obtener_cantidad_reportes")]
    [Description("Cuenta reportes: hoy, esta semana y total en el sistema.")]
    public async Task<string> ObtenerCantidadReportesAsync()
    {
        var cacheKey = "safebot:reportes:cantidad";
        if (_redis.IsEnabled)
        {
            var cached = await _redis.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached)) return cached;
        }

        var hoy = DateTime.UtcNow.Date;
        var semana = DateTime.UtcNow.AddDays(-7);
        var total = await _db.Reportes.CountAsync();
        var hoyCount = await _db.Reportes.CountAsync(r => r.FechaReporte >= hoy);
        var semanaCount = await _db.Reportes.CountAsync(r => r.FechaReporte >= semana);
        var aprobados = await _db.Reportes.CountAsync(r => r.Estado == "Aprobado");
        var pendientes = await _db.Reportes.CountAsync(r => r.Estado == "Pendiente");

        var json = JsonSerializer.Serialize(new
        {
            total,
            hoy = hoyCount,
            ultima_semana = semanaCount,
            aprobados,
            pendientes,
        });

        if (_redis.IsEnabled)
            await _redis.SetStringAsync(cacheKey, json, TimeSpan.FromMinutes(3));

        return json;
    }

    [KernelFunction("obtener_reportes_por_zona")]
    [Description("Lista reportes filtrados por nombre de zona o ubicación.")]
    public async Task<string> ObtenerReportesPorZonaAsync(
        [Description("Nombre de zona o barrio, ej. Bellavista, Miraflores")] string zona,
        int max = 8)
    {
        var n = Math.Clamp(max, 1, 20);
        var term = (zona ?? "").Trim();
        if (term.Length < 2)
            return JsonSerializer.Serialize(new { error = "Indica una zona válida." });

        var cacheKey = $"safebot:reportes:zona:{term.ToLowerInvariant()}:{n}";
        if (_redis.IsEnabled)
        {
            var cached = await _redis.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached)) return cached;
        }

        var items = await _db.Reportes
            .AsNoTracking()
            .Where(r => r.Estado != "Rechazado"
                        && r.Ubicacion.Contains(term))
            .OrderByDescending(r => r.FechaReporte)
            .Take(n)
            .Select(r => new
            {
                r.TipoIncidente,
                r.Ubicacion,
                r.Estado,
                r.FechaReporte,
            })
            .ToListAsync();

        var json = JsonSerializer.Serialize(new
        {
            zona = term,
            cantidad = items.Count,
            reportes = items,
        });

        if (_redis.IsEnabled)
            await _redis.SetStringAsync(cacheKey, json, TimeSpan.FromMinutes(5));

        return json;
    }
}
