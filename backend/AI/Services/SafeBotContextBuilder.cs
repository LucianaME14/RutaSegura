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

    public SafeBotContextBuilder(
        ReportesPlugin reportes,
        RutasPlugin rutas,
        AlertasPlugin alertas,
        MlPlugin ml,
        ClimaPlugin clima,
        AdminPlugin admin)
    {
        _reportes = reportes;
        _rutas = rutas;
        _alertas = alertas;
        _ml = ml;
        _clima = clima;
        _admin = admin;
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

        if (Matches(lower, "sos", "emergencia", "ayuda", "peligro", "robaron", "asalto"))
        {
            sb.AppendLine("[[SOS]] Botón SOS en el mapa alerta contactos de emergencia y registra ubicación.");
            fuentes.Add("Sistema SOS");
        }

        if (Matches(lower, "reporte", "reportes", "incidente", "cerca", "cercanos", "hoy", "cantidad", "cuántos", "cuantos"))
        {
            sb.AppendLine(await _reportes.ObtenerCantidadReportesAsync());
            fuentes.Add("PluginReportes");

            if (Matches(lower, "cerca", "reciente", "cercanos"))
            {
                sb.AppendLine(await _reportes.ObtenerReportesRecientesAsync(6));
            }

            if (!string.IsNullOrWhiteSpace(zona) || Matches(lower, "zona", "barrio", "avenida"))
            {
                var z = zona ?? ExtractZona(lower) ?? "Lima";
                sb.AppendLine(await _reportes.ObtenerReportesPorZonaAsync(z));
            }
        }

        if (Matches(lower, "segura", "seguro", "riesgo", "peligro", "clasificar", "zona"))
        {
            var z = zona ?? ExtractZona(lower) ?? "Lima";
            sb.AppendLine(await _ml.ClasificarZonaAsync(z));
            sb.AppendLine(await _alertas.ObtenerNivelRiesgoAsync(z));
            fuentes.Add("PluginML");
            fuentes.Add("PluginAlertas");
        }

        if (Matches(lower, "ruta", "caminar", "ir a", "universidad", "destino", "recomend"))
        {
            var o = origen ?? "Mi ubicación";
            var d = destino ?? ExtractDestino(lower) ?? "Destino";
            if (Matches(lower, "rápida", "rapida", "rápido", "rapido"))
                sb.AppendLine(await _rutas.ObtenerRutaMasRapidaAsync(userId, o, d));
            else if (Matches(lower, "equilibrad"))
                sb.AppendLine(await _rutas.ObtenerRutaEquilibradaAsync(userId, o, d));
            else
                sb.AppendLine(await _rutas.ObtenerRutaMasSeguraAsync(userId, o, d));

            sb.AppendLine(await _ml.RecomendarRutaAsync(userId, o, d));
            fuentes.Add("PluginRutas");
            fuentes.Add("PluginML");
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

        if (esAdmin && Matches(lower, "admin", "usuario", "registrado", "sistema", "resumen", "dashboard", "estadística", "estadistica"))
        {
            sb.AppendLine(await _admin.ResumenSistemaAsync());
            fuentes.Add("AdminPlugin");
        }

        if (esAdmin && Matches(lower, "peligros", "incidentes", "zonas"))
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
        foreach (var z in new[] { "miraflores", "san isidro", "barranco", "bellavista", "surco", "lima centro" })
        {
            if (lower.Contains(z, StringComparison.OrdinalIgnoreCase))
                return z;
        }
        return null;
    }

    private static string? ExtractDestino(string lower)
    {
        if (lower.Contains("universidad", StringComparison.OrdinalIgnoreCase))
            return "Universidad";
        return null;
    }
}
