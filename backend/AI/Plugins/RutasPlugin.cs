using System.ComponentModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using RutaSegura.Data;
using RutaSegura.Services;

namespace RutaSegura.AI.Plugins;

/// <summary>Plugin Semantic Kernel: recomendación de rutas (ML.NET + historial SQLite).</summary>
public class RutasPlugin
{
    private readonly MlNetService _ml;
    private readonly ApplicationDbContext _db;
    private readonly RedisService _redis;

    public RutasPlugin(MlNetService ml, ApplicationDbContext db, RedisService redis)
    {
        _ml = ml;
        _db = db;
        _redis = redis;
    }

    [KernelFunction("obtener_ruta_mas_segura")]
    [Description("Recomienda la ruta más segura entre origen y destino usando ML.NET.")]
    public async Task<string> ObtenerRutaMasSeguraAsync(
        int usuarioId,
        string origen = "Mi ubicación",
        string destino = "Destino")
    {
        return await RecomendarAsync(usuarioId, origen, destino, "segura");
    }

    [KernelFunction("obtener_ruta_mas_rapida")]
    [Description("Recomienda la ruta más rápida entre origen y destino.")]
    public async Task<string> ObtenerRutaMasRapidaAsync(
        int usuarioId,
        string origen = "Mi ubicación",
        string destino = "Destino")
    {
        return await RecomendarAsync(usuarioId, origen, destino, "rapida");
    }

    [KernelFunction("obtener_ruta_equilibrada")]
    [Description("Recomienda una ruta equilibrada entre seguridad y tiempo.")]
    public async Task<string> ObtenerRutaEquilibradaAsync(
        int usuarioId,
        string origen = "Mi ubicación",
        string destino = "Destino")
    {
        return await RecomendarAsync(usuarioId, origen, destino, "equilibrada");
    }

    private async Task<string> RecomendarAsync(
        int usuarioId,
        string origen,
        string destino,
        string perfil)
    {
        var cacheKey =
            $"safebot:ruta:{usuarioId}:{perfil}:{origen.Trim().ToLowerInvariant()}:{destino.Trim().ToLowerInvariant()}";
        if (_redis.IsEnabled)
        {
            var cached = await _redis.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached)) return cached;
        }

        await _ml.EnsureModelsAsync();
        var perfiles = _ml.RecommendRouteProfiles(usuarioId, origen, destino);
        var elegido = perfiles.FirstOrDefault(p => p.VarianteId == perfil)
                      ?? perfiles.FirstOrDefault();

        var historial = await _db.RutasHistorial
            .AsNoTracking()
            .Where(r => r.UsuarioId == usuarioId)
            .OrderByDescending(r => r.CreadoEn)
            .Take(3)
            .Select(r => new { r.OrigenTexto, r.DestinoTexto, r.Modo, r.MinutosAprox })
            .ToListAsync();

        var json = JsonSerializer.Serialize(new
        {
            perfil,
            origen,
            destino,
            recomendacion = elegido == null
                ? null
                : new
                {
                    elegido.Nombre,
                    elegido.ModoSugerido,
                    elegido.SeguridadPct,
                    elegido.PreferenceScore,
                },
            alternativas = perfiles.Select(p => new
            {
                p.VarianteId,
                p.Nombre,
                p.SeguridadPct,
            }),
            historial_reciente = historial,
            motor = "ML.NET Matrix Factorization",
        });

        if (_redis.IsEnabled)
            await _redis.SetStringAsync(cacheKey, json, TimeSpan.FromMinutes(8));

        return json;
    }
}
