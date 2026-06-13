using System.Text.Json;
using System.Text.RegularExpressions;

namespace RutaSegura.AI.Services;

/// <summary>Respuestas legibles cuando no hay LLM (Render / Ollama apagado).</summary>
public static class SafeBotFallbackFormatter
{
    public static string Format(string message, string context)
    {
        var lower = message.ToLowerInvariant();

        if (Matches(lower, "sos", "emergencia", "robaron") && !Matches(lower, "reporte"))
            return Prompts.SafeBotPrompts.FallbackSos;

        if (Matches(lower, "reportar", "hueco"))
            return Prompts.SafeBotPrompts.FallbackReportar;

        foreach (var json in ExtractJsonBlocks(context))
        {
            if (LooksLikeExplicacion(json))
                return FormatExplicacion(json);

            if (Matches(lower, "ruta", "caminar", "ir a", "universidad", "recomend", "por qué", "porque") && LooksLikeRuta(json))
                return FormatRuta(json, lower);

            if (Matches(lower, "reporte", "incidente", "cerca", "cercanos", "hoy", "cuántos", "cuantos")
                && LooksLikeReportes(json))
                return FormatReportes(json, lower);

            if (Matches(lower, "alerta", "alertas", "aviso") && LooksLikeAlertas(json))
                return FormatAlertas(json);

            if (Matches(lower, "clima", "lluvia", "tiempo", "temperatura") && LooksLikeClima(json))
                return FormatClima(json);

            if (Matches(lower, "admin", "usuario", "registrado", "resumen", "sistema", "dashboard")
                && LooksLikeAdmin(json))
                return FormatAdmin(json);
        }

        foreach (var json in ExtractJsonBlocks(context))
        {
            if (LooksLikeExplicacion(json))
                return FormatExplicacion(json);

            if (LooksLikeZona(json))
                return FormatZona(json);

            if (LooksLikeRuta(json))
                return FormatRuta(json, lower);

            if (LooksLikeReportes(json))
                return FormatReportes(json, lower);
        }

        return "Puedo ayudarte con zonas seguras, rutas, reportes, clima, SOS y (si eres admin) resúmenes del sistema. "
               + "Prueba: «¿Por qué Miraflores es moderada?» o «¿Cuál es la ruta más segura?».";
    }

    private static string FormatExplicacion(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var msg = GetStr(root, "mensaje");
        if (!string.IsNullOrEmpty(msg))
            return DecodeUnicode(msg);

        var tipo = GetStr(root, "tipo") ?? "";
        if (tipo == "tendencia_zona")
        {
            var zona = GetStr(root, "zona") ?? "la zona";
            var tend = GetStr(root, "tendencia") ?? "estable";
            var act = root.TryGetProperty("reportes_semana_actual", out var a) ? a.GetInt32() : 0;
            var ant = root.TryGetProperty("reportes_semana_anterior", out var b) ? b.GetInt32() : 0;
            return $"En **{zona}** el riesgo **{tend}**: {act} reportes esta semana vs {ant} la semana anterior.";
        }

        if (tipo == "zonas_criticas")
            return msg ?? "Consulté las zonas con más incidentes en el sistema.";

        if (tipo == "explicacion_ruta")
        {
            var dest = GetStr(root, "destino") ?? "tu destino";
            var ruta = GetStr(root, "ruta_recomendada") ?? "la ruta recomendada";
            return $"Hacia **{dest}**, **{ruta}** es la mejor opción según ML.NET, historial y condiciones actuales.";
        }

        if (tipo == "explicacion_zona" && root.TryGetProperty("factores", out var factores))
        {
            var zona = GetStr(root, "zona") ?? "la zona";
            var nivel = GetStr(root, "nivel") ?? "moderada";
            var lista = factores.EnumerateArray().Select(f => f.GetString()).Where(s => s != null);
            return $"**{zona}** es **{nivel}** porque: {string.Join("; ", lista)}.";
        }

        return "He analizado los datos. Revisa el mapa para más detalle.";
    }

    private static bool LooksLikeExplicacion(string json) =>
        json.Contains("\"tipo\":\"explicacion", StringComparison.OrdinalIgnoreCase)
        || json.Contains("explicacion_zona", StringComparison.OrdinalIgnoreCase)
        || json.Contains("explicacion_ruta", StringComparison.OrdinalIgnoreCase)
        || json.Contains("tendencia_zona", StringComparison.OrdinalIgnoreCase)
        || json.Contains("zonas_criticas", StringComparison.OrdinalIgnoreCase)
        || json.Contains("\"mensaje\":", StringComparison.OrdinalIgnoreCase) && json.Contains("factores", StringComparison.OrdinalIgnoreCase);

    private static string FormatZona(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var zona = GetStr(root, "zona") ?? "la zona";
        var msg = GetStr(root, "mensaje_tecnico");
        if (!string.IsNullOrEmpty(msg))
            return DecodeUnicode(msg);

        var riesgo = GetStr(root, "riesgo") ?? GetStr(root, "clasificacion") ?? "moderado";
        var conf = root.TryGetProperty("confianza_pct", out var c)
            ? $"{c.GetDouble():F0}%"
            : root.TryGetProperty("confianza", out var cf)
                ? $"{cf.GetDouble() * 100:F0}%"
                : "";

        return conf.Length > 0
            ? $"Según ML.NET y los reportes, **{zona}** presenta riesgo **{riesgo}** (confianza {conf}). "
              + "Factores: incidentes recientes, horario y datos históricos de la zona."
            : $"Según los datos del sistema, **{zona}** presenta riesgo **{riesgo}**.";
    }

    private static string FormatRuta(string json, string lower)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var destino = GetStr(root, "destino") ?? "tu destino";
        var perfil = GetStr(root, "perfil") ?? "segura";

        if (root.TryGetProperty("recomendacion", out var rec) && rec.ValueKind == JsonValueKind.Object)
        {
            var nombre = GetStr(rec, "Nombre") ?? GetStr(rec, "nombre") ?? "Ruta recomendada";
            var seg = rec.TryGetProperty("SeguridadPct", out var s)
                ? s.GetDouble()
                : rec.TryGetProperty("seguridadPct", out var s2) ? s2.GetDouble() : 0;
            var tipo = Matches(lower, "rápida", "rapida") ? "más rápida"
                : Matches(lower, "equilibrad") ? "equilibrada" : "más segura";
            return $"La ruta **{tipo}** hacia **{destino}** es **{nombre}** (seguridad {seg:F0}%). "
                   + "ML.NET la prioriza por menor historial de incidentes y mejor puntuación en tu perfil. "
                   + "Ver recorrido en **Buscar Ruta**.";
        }

        if (root.TryGetProperty("mejor_nombre", out var mn))
        {
            var seg = root.TryGetProperty("seguridad_pct", out var sp) ? sp.GetDouble() : 0;
            return $"Recomendación ML.NET hacia **{destino}**: **{mn.GetString()}** (seguridad {seg:F0}%).";
        }

        return $"Consulté rutas hacia **{destino}** (perfil {perfil}). Usa **Buscar Ruta** en el mapa para ver alternativas.";
    }

    private static string FormatReportes(string json, string lower)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.TryGetProperty("reportes", out var arr) && arr.ValueKind == JsonValueKind.Array)
        {
            var zona = GetStr(root, "zona");
            var items = arr.EnumerateArray().ToList();
            if (items.Count == 0)
            {
                var refZona = zona ?? "esa zona";
                return $"No hay reportes recientes registrados cerca de **{refZona}** en los últimos 30 días.";
            }

            var lines = items.Take(5).Select(r =>
            {
                var tipo = GetStr(r, "tipo") ?? GetStr(r, "TipoIncidente") ?? "Incidente";
                var ubic = GetStr(r, "ubicacion") ?? GetStr(r, "Ubicacion") ?? "";
                var hace = GetStr(r, "hace") ?? "";
                var estado = GetStr(r, "estado") ?? GetStr(r, "Estado") ?? "";
                return $"• **{tipo}** en {ubic} ({estado}{(hace.Length > 0 ? $", hace {hace}" : "")})";
            });

            var header = zona != null
                ? $"Reportes cerca de **{zona}** ({items.Count}):"
                : $"Hay **{items.Count}** reporte(s) reciente(s):";
            return header + "\n" + string.Join("\n", lines);
        }

        if (root.TryGetProperty("hoy", out var hoy))
        {
            var total = root.TryGetProperty("total", out var t) ? t.GetInt32() : 0;
            return $"Hoy hay **{hoy.GetInt32()}** reporte(s). En total en el sistema: **{total}** "
                   + $"({(root.TryGetProperty("aprobados", out var a) ? a.GetInt32() : 0)} aprobados, "
                   + $"{(root.TryGetProperty("pendientes", out var p) ? p.GetInt32() : 0)} pendientes).";
        }

        return "Consulté los reportes del sistema. Revisa el mapa para ver incidentes en tu zona.";
    }

    private static string FormatAlertas(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var count = 0;
        if (root.TryGetProperty("alertas_sistema", out var a) && a.ValueKind == JsonValueKind.Array)
            count += a.GetArrayLength();
        if (root.TryGetProperty("reportes_destacados", out var r) && r.ValueKind == JsonValueKind.Array)
            count += r.GetArrayLength();

        return count > 0
            ? $"Hay **{count}** alerta(s) o reporte(s) destacado(s) activos. Revisa el mapa y la sección de alertas."
            : "No hay alertas críticas activas en este momento.";
    }

    private static string FormatClima(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var temp = root.TryGetProperty("TemperaturaC", out var tc) ? tc.GetDouble()
            : root.TryGetProperty("temperaturaC", out var tc2) ? tc2.GetDouble() : 0;
        var desc = GetStr(root, "Descripcion") ?? GetStr(root, "descripcion") ?? "despejado";
        return $"Clima actual en Lima: **{temp:F0}°C**, {desc}. Tenlo en cuenta al elegir tu ruta.";
    }

    private static string FormatAdmin(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var usuarios = root.TryGetProperty("usuarios_registrados", out var u) ? u.GetInt32() : 0;
        var hoy = root.TryGetProperty("reportes_hoy", out var h) ? h.GetInt32() : 0;
        var pend = root.TryGetProperty("reportes_pendientes", out var p) ? p.GetInt32() : 0;
        var zonas = "";
        if (root.TryGetProperty("zonas_mas_incidentes", out var zArr) && zArr.ValueKind == JsonValueKind.Array)
        {
            var top = zArr.EnumerateArray().Take(3)
                .Select(z => $"{GetStr(z, "zona") ?? "?"} ({(z.TryGetProperty("cantidad", out var c) ? c.GetInt32() : 0)})");
            zonas = string.Join(", ", top);
        }
        return $"**Resumen:** {usuarios} usuarios, **{hoy}** reportes hoy, **{pend}** pendientes."
               + (zonas.Length > 0 ? $" Zonas con más incidentes: {zonas}." : "");
    }

    private static IEnumerable<string> ExtractJsonBlocks(string context)
    {
        foreach (Match m in Regex.Matches(context, @"\{[^{}]*(?:\{[^{}]*\}[^{}]*)*\}"))
        {
            var s = m.Value.Trim();
            if (s.StartsWith('{') && s.EndsWith('}'))
                yield return s;
        }
    }

    private static bool LooksLikeZona(string json) =>
        json.Contains("clasificacion", StringComparison.OrdinalIgnoreCase)
        || json.Contains("mensaje_tecnico", StringComparison.OrdinalIgnoreCase);

    private static bool LooksLikeRuta(string json) =>
        json.Contains("perfil", StringComparison.OrdinalIgnoreCase)
        || json.Contains("recomendacion", StringComparison.OrdinalIgnoreCase)
        || json.Contains("mejor_perfil", StringComparison.OrdinalIgnoreCase);

    private static bool LooksLikeReportes(string json) =>
        json.Contains("\"reportes\"", StringComparison.OrdinalIgnoreCase)
        || (json.Contains("\"hoy\"", StringComparison.OrdinalIgnoreCase) && json.Contains("\"total\"", StringComparison.OrdinalIgnoreCase));

    private static bool LooksLikeAlertas(string json) =>
        json.Contains("alertas_sistema", StringComparison.OrdinalIgnoreCase);

    private static bool LooksLikeClima(string json) =>
        json.Contains("TemperaturaC", StringComparison.OrdinalIgnoreCase)
        || json.Contains("temperaturaC", StringComparison.OrdinalIgnoreCase);

    private static bool LooksLikeAdmin(string json) =>
        json.Contains("usuarios_registrados", StringComparison.OrdinalIgnoreCase);

    private static string? GetStr(JsonElement el, string prop) =>
        el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String
            ? v.GetString()
            : null;

    private static string DecodeUnicode(string s) =>
        s.Contains("\\u", StringComparison.Ordinal) ? Regex.Unescape(s) : s;

    private static bool Matches(string text, params string[] keywords) =>
        keywords.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
}
