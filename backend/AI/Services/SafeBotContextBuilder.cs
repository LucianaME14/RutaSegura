using System.Text;
using RutaSegura.AI.Plugins;

namespace RutaSegura.AI.Services;

/// <summary>Recolecta datos de plugins según la intención del mensaje.</summary>
public class SafeBotContextBuilder
{
    private readonly ReportesPlugin _reportes;
    private readonly RutasPlugin _rutas;
    private readonly AlertasPlugin _alertas;
    private readonly MlPlugin _ml;
    private readonly ClimaPlugin _clima;
    private readonly AdminPlugin _admin;
    private readonly ExplicacionesPlugin _explicaciones;

    public SafeBotContextBuilder(
        ReportesPlugin reportes,
        RutasPlugin rutas,
        AlertasPlugin alertas,
        MlPlugin ml,
        ClimaPlugin clima,
        AdminPlugin admin,
        ExplicacionesPlugin explicaciones)
    {
        _reportes = reportes;
        _rutas = rutas;
        _alertas = alertas;
        _ml = ml;
        _clima = clima;
        _admin = admin;
        _explicaciones = explicaciones;
    }

    public async Task<(string Context, List<string> Fuentes)> BuildAsync(
        string message,
        int userId,
        bool esAdmin,
        string? zona,
        string? origen,
        string? destino,
        CancellationToken ct = default)
    {
        var lower = message.ToLowerInvariant();
        var sb = new StringBuilder();
        var fuentes = new List<string>();

        var quiereExplicacion = Matches(lower, "por qué", "porque", "porq", "explica", "motivo", "razón", "razon", "por que");
        var ubicacion = zona ?? ExtractUbicacion(lower) ?? ExtractZona(lower);
        var esPreguntaRuta = Matches(lower, "ruta", "caminar", "ir a", "universidad", "destino", "recomend");

        if (Matches(lower, "sos", "emergencia", "ayuda", "peligro", "robaron", "asalto"))
        {
            sb.AppendLine("[[SOS]] Botón SOS en el mapa alerta contactos de emergencia y registra ubicación.");
            fuentes.Add("Sistema SOS");
        }

        // Admin: zonas críticas hoy / resumen
        if (esAdmin && Matches(lower, "crític", "critic", "resume", "resumen", "hoy", "zonas", "peligros", "sistema", "dashboard"))
        {
            if (Matches(lower, "crític", "critic", "zonas", "hoy", "resume", "resumen"))
            {
                sb.AppendLine(await _explicaciones.ZonasCriticasHoyAsync());
                fuentes.Add("ExplicacionesPlugin");
            }
            sb.AppendLine(await _admin.ResumenSistemaAsync());
            fuentes.Add("AdminPlugin");
        }

        // Admin: tendencia / por qué subió el riesgo
        if (esAdmin && Matches(lower, "subió", "subio", "sube", "tendencia", "aumentó", "aumento", "bajó", "bajo", "cambió", "cambio")
            || (quiereExplicacion && Matches(lower, "riesgo", "subió", "subio", "aument")))
        {
            var z = ubicacion ?? "Lima";
            sb.AppendLine(await _explicaciones.AnalizarTendenciaZonaAsync(z));
            sb.AppendLine(await _explicaciones.ExplicarZonaAsync(z));
            fuentes.Add("ExplicacionesPlugin");
        }

        if (Matches(lower, "reporte", "reportes", "incidente", "cerca", "cercanos", "cuántos", "cuantos")
            || (Matches(lower, "hoy", "cantidad") && !esAdmin))
        {
            sb.AppendLine(await _reportes.ObtenerCantidadReportesAsync());
            fuentes.Add("PluginReportes");

            if (!string.IsNullOrWhiteSpace(ubicacion))
                sb.AppendLine(await _reportes.ObtenerReportesPorZonaAsync(ubicacion));
            else if (Matches(lower, "cerca", "reciente", "cercanos"))
                sb.AppendLine(await _reportes.ObtenerReportesRecientesAsync(6));
        }

        // Explicar zona (usuario o admin)
        if (quiereExplicacion && !esPreguntaRuta
            || (!esPreguntaRuta && Matches(lower, "segura", "seguro", "riesgo", "peligrosa", "peligroso", "clasificar", "zona", "moderada", "miraflores", "barranco")))
        {
            var z = ubicacion ?? "Lima";
            sb.AppendLine(await _explicaciones.ExplicarZonaAsync(z));
            sb.AppendLine(await _ml.ClasificarZonaAsync(z));
            sb.AppendLine(await _alertas.ObtenerNivelRiesgoAsync(z));
            fuentes.Add("ExplicacionesPlugin");
            fuentes.Add("PluginML");
            fuentes.Add("PluginAlertas");
        }

        if (esPreguntaRuta)
        {
            var o = origen ?? "Mi ubicación";
            var d = destino ?? ExtractDestino(lower) ?? "Destino";
            var perfil = Matches(lower, "rápida", "rapida", "rápido", "rapido") ? "rapida"
                : Matches(lower, "equilibrad") ? "equilibrada" : "segura";

            if (quiereExplicacion || Matches(lower, "por qué", "porque", "motivo"))
                sb.AppendLine(await _explicaciones.ExplicarRutaAsync(userId, o, d, perfil));
            else if (perfil == "rapida")
                sb.AppendLine(await _rutas.ObtenerRutaMasRapidaAsync(userId, o, d));
            else if (perfil == "equilibrada")
                sb.AppendLine(await _rutas.ObtenerRutaEquilibradaAsync(userId, o, d));
            else
                sb.AppendLine(await _rutas.ObtenerRutaMasSeguraAsync(userId, o, d));

            sb.AppendLine(await _ml.RecomendarRutaAsync(userId, o, d));
            fuentes.Add("PluginRutas");
            fuentes.Add("PluginML");
            if (quiereExplicacion) fuentes.Add("ExplicacionesPlugin");
        }

        if (Matches(lower, "alerta", "alertas", "aviso"))
        {
            sb.AppendLine(await _alertas.ObtenerAlertasActivasAsync());
            fuentes.Add("PluginAlertas");
        }

        if (Matches(lower, "clima", "lluvia", "tiempo", "temperatura"))
        {
            sb.AppendLine(await _clima.ObtenerClimaActualAsync());
            fuentes.Add("PluginClima");
        }

        if (esAdmin && Matches(lower, "usuario", "registrado", "admin", "estadística", "estadistica")
            && !Matches(lower, "crític", "critic", "resume", "resumen"))
        {
            sb.AppendLine(await _admin.ResumenSistemaAsync());
            fuentes.Add("AdminPlugin");
        }

        if (sb.Length == 0)
        {
            sb.AppendLine(await _reportes.ObtenerReportesRecientesAsync(3));
            sb.AppendLine(await _alertas.ObtenerAlertasActivasAsync(3));
            fuentes.Add("PluginReportes");
            fuentes.Add("PluginAlertas");
        }

        return (sb.ToString().Trim(), fuentes.Distinct().ToList());
    }

    private static bool Matches(string text, params string[] keywords) =>
        keywords.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));

    private static string? ExtractZona(string lower)
    {
        foreach (var z in new[] { "miraflores", "san isidro", "barranco", "bellavista", "surco", "lima centro", "la molina", "molina" })
        {
            if (lower.Contains(z, StringComparison.OrdinalIgnoreCase))
                return z;
        }
        return null;
    }

    private static string? ExtractUbicacion(string lower)
    {
        foreach (var marker in new[] { "cerca de ", "cerca del ", "cerca de la ", "en ", "por ", "zona " })
        {
            var idx = lower.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) continue;
            var rest = lower[(idx + marker.Length)..].Trim().TrimEnd('?', '.', '!');
            if (rest.Length >= 2)
                return char.ToUpper(rest[0]) + rest[1..];
        }

        foreach (var place in new[] { "usmp", "fia", "universidad", "barranco", "miraflores", "molina", "san isidro", "surco" })
        {
            if (lower.Contains(place, StringComparison.OrdinalIgnoreCase))
                return place.Equals("usmp", StringComparison.OrdinalIgnoreCase) ? "USMP"
                    : place.Equals("fia", StringComparison.OrdinalIgnoreCase) ? "FIA"
                    : char.ToUpper(place[0]) + place[1..];
        }

        return ExtractZona(lower);
    }

    private static string? ExtractDestino(string lower)
    {
        if (lower.Contains("universidad", StringComparison.OrdinalIgnoreCase)
            || lower.Contains("usmp", StringComparison.OrdinalIgnoreCase))
            return "Universidad USMP";
        return null;
    }
}
