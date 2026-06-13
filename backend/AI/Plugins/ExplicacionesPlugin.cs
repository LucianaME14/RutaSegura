using System.ComponentModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using RutaSegura.AI.Plugins;
using RutaSegura.Data;
using RutaSegura.Services;

namespace RutaSegura.AI.Plugins;

/// <summary>Explicaciones detalladas: por qué una zona o ruta, tendencias y zonas críticas.</summary>
public class ExplicacionesPlugin
{
    private readonly ApplicationDbContext _db;
    private readonly MlPlugin _ml;
    private readonly RutasPlugin _rutas;
    private readonly ClimaPlugin _clima;
    private readonly AlertasPlugin _alertas;

    public ExplicacionesPlugin(
        ApplicationDbContext db,
        MlPlugin ml,
        RutasPlugin rutas,
        ClimaPlugin clima,
        AlertasPlugin alertas)
    {
        _db = db;
        _ml = ml;
        _rutas = rutas;
        _clima = clima;
        _alertas = alertas;
    }

    [KernelFunction("explicar_zona")]
    [Description("Explica por qué una zona es segura, moderada o peligrosa con factores concretos.")]
    public async Task<string> ExplicarZonaAsync(string zona = "Lima")
    {
        var term = (zona ?? "Lima").Trim();
        var clasificacion = await _ml.ClasificarZonaAsync(term);
        var riesgo = await _alertas.ObtenerNivelRiesgoAsync(term);
        var clima = await _clima.ObtenerClimaActualAsync();

        var desde30 = DateTime.UtcNow.AddDays(-30);
        var reportes = await _db.Reportes.AsNoTracking()
            .Where(r => r.Ubicacion.Contains(term) && r.FechaReporte >= desde30 && r.Estado != "Rechazado")
            .ToListAsync();

        var graves = reportes.Count(r => r.TipoIncidente is "Robo" or "Asalto");
        var nocturnos = reportes.Count(r => r.FechaReporte.Hour >= 19 || r.FechaReporte.Hour < 6);
        var hora = DateTime.Now.Hour;
        var esNoche = hora >= 19 || hora < 6;

        var factores = new List<string>();
        if (reportes.Count >= 10)
            factores.Add($"{reportes.Count} reportes en los últimos 30 días");
        else if (reportes.Count > 0)
            factores.Add($"{reportes.Count} reporte(s) reciente(s) en la zona");
        else
            factores.Add("Pocos o ningún reporte reciente en la zona");

        if (graves > 0)
            factores.Add($"{graves} incidente(s) grave(s) (robo/asalto)");
        if (nocturnos > 0 && esNoche)
            factores.Add("Horario nocturno con incidentes previos de noche");
        else if (esNoche)
            factores.Add("Es de noche: mayor precaución recomendada");

        using var climaDoc = JsonDocument.Parse(clima);
        var descClima = climaDoc.RootElement.TryGetProperty("Descripcion", out var d)
            ? d.GetString() : climaDoc.RootElement.TryGetProperty("descripcion", out var d2) ? d2.GetString() : "";
        if (descClima?.Contains("lluv", StringComparison.OrdinalIgnoreCase) == true)
            factores.Add("Clima con lluvia puede reducir visibilidad");

        using var clsDoc = JsonDocument.Parse(clasificacion);
        var nivel = clsDoc.RootElement.TryGetProperty("riesgo", out var r) ? r.GetString()
            : clsDoc.RootElement.TryGetProperty("clasificacion", out var c) ? c.GetString() : "Moderada";
        var conf = clsDoc.RootElement.TryGetProperty("confianza_pct", out var cp)
            ? cp.GetDouble() : 75;

        return JsonSerializer.Serialize(new
        {
            tipo = "explicacion_zona",
            zona = term,
            nivel,
            confianza_pct = conf,
            factores,
            reportes_30d = reportes.Count,
            incidentes_graves = graves,
            hora_actual = hora,
            es_noche = esNoche,
            clima = descClima,
            mensaje =
                $"**{term}** se clasifica como **{nivel}** (ML.NET, confianza {conf:F0}%). "
                + $"Factores: {string.Join("; ", factores)}.",
        });
    }

    [KernelFunction("explicar_ruta")]
    [Description("Explica por qué se recomienda una ruta segura, rápida o equilibrada.")]
    public async Task<string> ExplicarRutaAsync(
        int usuarioId,
        string origen = "Mi ubicación",
        string destino = "Destino",
        string perfil = "segura")
    {
        var rutaJson = perfil switch
        {
            "rapida" => await _rutas.ObtenerRutaMasRapidaAsync(usuarioId, origen, destino),
            "equilibrada" => await _rutas.ObtenerRutaEquilibradaAsync(usuarioId, origen, destino),
            _ => await _rutas.ObtenerRutaMasSeguraAsync(usuarioId, origen, destino),
        };

        using var doc = JsonDocument.Parse(rutaJson);
        var root = doc.RootElement;
        var nombre = "Ruta recomendada";
        var seg = 0.0;
        if (root.TryGetProperty("recomendacion", out var rec) && rec.ValueKind == JsonValueKind.Object)
        {
            nombre = rec.TryGetProperty("Nombre", out var n) ? n.GetString() ?? nombre
                : rec.TryGetProperty("nombre", out var n2) ? n2.GetString() ?? nombre : nombre;
            seg = rec.TryGetProperty("SeguridadPct", out var s) ? s.GetDouble()
                : rec.TryGetProperty("seguridadPct", out var s2) ? s2.GetDouble() : 0;
        }

        var clima = await _clima.ObtenerClimaActualAsync();
        using var climaDoc = JsonDocument.Parse(clima);
        var impacto = climaDoc.RootElement.TryGetProperty("impacto_movilidad", out var im)
            ? im.GetString() : "";

        var razones = new List<string>
        {
            $"ML.NET prioriza el perfil **{perfil}** según historial y datos de rutas",
            $"Puntuación de seguridad estimada: {seg:F0}%",
            "Google Directions traza el recorrido sobre calles reales",
        };
        if (!string.IsNullOrEmpty(impacto) && impacto.Contains("afect", StringComparison.OrdinalIgnoreCase))
            razones.Add("El clima actual favorece priorizar la ruta más segura");

        return JsonSerializer.Serialize(new
        {
            tipo = "explicacion_ruta",
            origen,
            destino,
            perfil,
            ruta_recomendada = nombre,
            seguridad_pct = seg,
            razones,
            mensaje =
                $"La ruta **{nombre}** hacia **{destino}** es la más **{perfil}** porque: "
                + string.Join("; ", razones) + ".",
        });
    }

    [KernelFunction("analizar_tendencia_zona")]
    [Description("Analiza si el riesgo de una zona subió o bajó comparando la última semana con la anterior.")]
    public async Task<string> AnalizarTendenciaZonaAsync(string zona = "Lima")
    {
        var term = (zona ?? "Lima").Trim();
        var ahora = DateTime.UtcNow;
        var semanaActualInicio = ahora.AddDays(-7);
        var semanaAnteriorInicio = ahora.AddDays(-14);

        var actual = await _db.Reportes.AsNoTracking()
            .CountAsync(r => r.Ubicacion.Contains(term)
                             && r.FechaReporte >= semanaActualInicio
                             && r.Estado != "Rechazado");

        var anterior = await _db.Reportes.AsNoTracking()
            .CountAsync(r => r.Ubicacion.Contains(term)
                             && r.FechaReporte >= semanaAnteriorInicio
                             && r.FechaReporte < semanaActualInicio
                             && r.Estado != "Rechazado");

        var diff = actual - anterior;
        var tendencia = diff switch
        {
            > 2 => "subió",
            < -1 => "bajó",
            _ => "se mantiene estable",
        };

        var gravesActual = await _db.Reportes.AsNoTracking()
            .CountAsync(r => r.Ubicacion.Contains(term)
                             && r.FechaReporte >= semanaActualInicio
                             && r.Estado != "Rechazado"
                             && (r.TipoIncidente == "Robo" || r.TipoIncidente == "Asalto"));

        return JsonSerializer.Serialize(new
        {
            tipo = "tendencia_zona",
            zona = term,
            reportes_semana_actual = actual,
            reportes_semana_anterior = anterior,
            diferencia = diff,
            tendencia,
            incidentes_graves_semana = gravesActual,
            mensaje = diff > 0
                ? $"El riesgo en **{term}** **{tendencia}**: {actual} reportes esta semana vs {anterior} la semana anterior (+{diff})."
                : diff < 0
                    ? $"El riesgo en **{term}** **{tendencia}**: {actual} reportes esta semana vs {anterior} la anterior ({diff})."
                    : $"El riesgo en **{term}** **{tendencia}** con {actual} reportes esta semana.",
        });
    }

    [KernelFunction("zonas_criticas_hoy")]
    [Description("Resume las zonas más críticas del día para administradores.")]
    public async Task<string> ZonasCriticasHoyAsync()
    {
        var hoy = DateTime.UtcNow.Date;
        var reportesHoy = await _db.Reportes.AsNoTracking()
            .Where(r => r.FechaReporte >= hoy && r.Estado != "Rechazado")
            .Select(r => new { r.Ubicacion, r.TipoIncidente })
            .ToListAsync();

        var items = reportesHoy
            .GroupBy(r => r.Ubicacion)
            .Select(g => new { zona = g.Key, cantidad = g.Count(), tipos = g.Select(x => x.TipoIncidente).Distinct().ToList() })
            .OrderByDescending(x => x.cantidad)
            .Take(8)
            .ToList();

        if (items.Count == 0)
        {
            var ultimos7 = await _db.Reportes.AsNoTracking()
                .Where(r => r.FechaReporte >= DateTime.UtcNow.AddDays(-7) && r.Estado != "Rechazado")
                .GroupBy(r => r.Ubicacion)
                .Select(g => new { zona = g.Key, cantidad = g.Count() })
                .OrderByDescending(x => x.cantidad)
                .Take(5)
                .ToListAsync();

            return JsonSerializer.Serialize(new
            {
                tipo = "zonas_criticas",
                periodo = "ultimos_7_dias",
                zonas = ultimos7,
                mensaje = "Hoy no hay reportes nuevos. Zonas con más actividad en los últimos 7 días: "
                          + string.Join(", ", ultimos7.Select(z => $"{z.zona} ({z.cantidad})")),
            });
        }

        return JsonSerializer.Serialize(new
        {
            tipo = "zonas_criticas",
            periodo = "hoy",
            total_reportes_hoy = items.Sum(i => i.cantidad),
            zonas = items.Select(i => new { i.zona, i.cantidad, i.tipos }),
            mensaje = "**Zonas críticas hoy:** "
                      + string.Join("; ", items.Take(5).Select(z => $"{z.zona} ({z.cantidad} reporte(s))")),
        });
    }
}
