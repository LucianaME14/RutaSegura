using System.Globalization;
using System.Text.Json;
using RutaSegura.ML;

namespace RutaSegura.Services;

/// <summary>
/// Clasificación de zona en tiempo real (ML.NET) con caché Redis.
/// </summary>
public class MlZoneQueryService
{
    private readonly MlNetService _ml;
    private readonly RedisService _redis;
    private readonly ILogger<MlZoneQueryService> _logger;

    public MlZoneQueryService(MlNetService ml, RedisService redis, ILogger<MlZoneQueryService> logger)
    {
        _ml = ml;
        _redis = redis;
        _logger = logger;
    }

    public async Task<ZonaClasificacionResponse> ClasificarAsync(
        string zona,
        float cantidadReportes,
        float hora,
        float iluminacion,
        float trafico,
        float incidentesRecientes,
        CancellationToken ct = default)
    {
        var cacheKey =
            $"ml:zona:v1:{zona.Trim().ToLowerInvariant()}:{cantidadReportes:F2}:{hora:F1}:{iluminacion:F2}:{trafico:F2}:{incidentesRecientes:F2}";

        if (_redis.IsEnabled)
        {
            var cached = await _redis.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                var parsed = JsonSerializer.Deserialize<ZonaClasificacionResponse>(cached);
                if (parsed is not null)
                {
                    parsed = parsed with { ServidoDesdeCache = true, Motor = "ML.NET Model Builder (cache Redis)" };
                    return parsed;
                }
            }
        }

        await _ml.EnsureModelsAsync(ct);
        var result = _ml.ClassifyZoneSafety(
            cantidadReportes,
            trafico,
            iluminacion,
            hora,
            incidentesRecientes);

        var display = ZoneSafetyPresentation.ToDisplay(result.Nivel, result.ConfianzaPct);
        var response = new ZonaClasificacionResponse(
            Zona: zona.Trim(),
            Riesgo: ZoneSafetyPresentation.ToRiesgoEtiqueta(result.Nivel),
            Confianza: Math.Round(result.ConfianzaPct / 100.0, 2),
            IndicadorVisual: display.IndicadorVisual,
            Etiqueta: display.Etiqueta,
            Motor: "ML.NET Data Classification (SdcaMaximumEntropy)",
            ServidoDesdeCache: false);

        if (_redis.IsEnabled)
        {
            await _redis.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(response),
                TimeSpan.FromMinutes(15));
        }

        return response;
    }
}

public record ZonaClasificacionResponse(
    string Zona,
    string Riesgo,
    double Confianza,
    string IndicadorVisual,
    string Etiqueta,
    string Motor,
    bool ServidoDesdeCache = false);
