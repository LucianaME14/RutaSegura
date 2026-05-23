using System.Globalization;
using System.Text.Json;

namespace RutaSegura.Services;

/// <summary>
/// Integración con APIs externas (clima, geolocalización demo).
/// Configure OPENWEATHER_API_KEY en Render o .env para clima real.
/// </summary>
public class ExternalApisService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _config;
    private readonly RedisService _redis;
    private readonly ILogger<ExternalApisService> _logger;

    public ExternalApisService(
        IHttpClientFactory httpFactory,
        IConfiguration config,
        RedisService redis,
        ILogger<ExternalApisService> logger)
    {
        _httpFactory = httpFactory;
        _config = config;
        _redis = redis;
        _logger = logger;
    }

    public ApiCatalogResponse GetCatalog() =>
        new(
            [
                new ApiInfo(
                    "Google Maps JavaScript API",
                    "Mapas, marcadores, Places, geocodificación en el front.",
                    "VITE_GOOGLE_MAPS_API_KEY",
                    "Integrado en Mapa, Rutas, Mapa de calor admin."),
                new ApiInfo(
                    "OpenWeather API",
                    "Clima actual (temperatura, descripción) para evaluar condiciones nocturnas.",
                    "OPENWEATHER_API_KEY",
                    "GET /api/external/clima?lat=&lon="),
                new ApiInfo(
                    "OpenStreetMap / Nominatim (demo)",
                    "Geocodificación inversa vía backend /api/Geo/reverse.",
                    "Sin clave (uso moderado)",
                    "Reportar y Mapa"),
                new ApiInfo(
                    "Redis Cloud",
                    "Caché de sesiones, dashboard, ML y alertas.",
                    "Redis__ConnectionString",
                    "Toda la API"),
                new ApiInfo(
                    "ML.NET Model Builder",
                    "Clasificación de zonas y recomendación de rutas.",
                    "Datasets/*.csv → Models/*.zip",
                    "GET /api/ml/clasificar-zona, /api/ml/recomendar-rutas"),
            ]);

    public async Task<ClimaResponse> GetClimaAsync(double lat, double lon, CancellationToken ct = default)
    {
        var key = $"ext:clima:{lat:F3}:{lon:F3}";
        if (_redis.IsEnabled)
        {
            var cached = await _redis.GetStringAsync(key);
            if (!string.IsNullOrEmpty(cached))
            {
                var c = JsonSerializer.Deserialize<ClimaResponse>(cached);
                if (c is not null) return c with { Fuente = c.Fuente + " (Redis)" };
            }
        }

        var apiKey = _config["OPENWEATHER_API_KEY"]?.Trim();
        if (string.IsNullOrEmpty(apiKey))
        {
            return DemoClima(lat, lon);
        }

        try
        {
            var client = _httpFactory.CreateClient();
            var url =
                $"https://api.openweathermap.org/data/2.5/weather?lat={lat.ToString(CultureInfo.InvariantCulture)}&lon={lon.ToString(CultureInfo.InvariantCulture)}&appid={apiKey}&units=metric&lang=es";
            var json = await client.GetStringAsync(url, ct);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var desc = root.GetProperty("weather")[0].GetProperty("description").GetString() ?? "—";
            var temp = root.GetProperty("main").GetProperty("temp").GetDouble();
            var response = new ClimaResponse(
                Lat: lat,
                Lon: lon,
                TemperaturaC: Math.Round(temp, 1),
                Descripcion: desc,
                Fuente: "OpenWeather API");

            if (_redis.IsEnabled)
            {
                await _redis.SetStringAsync(key, JsonSerializer.Serialize(response), TimeSpan.FromMinutes(20));
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OpenWeather no disponible; usando demo");
            return DemoClima(lat, lon);
        }
    }

    public TraficoDemoResponse GetTraficoDemo(string zona)
    {
        var hash = Math.Abs(zona.GetHashCode()) % 100;
        var nivel = hash > 70 ? "Alto" : hash > 40 ? "Medio" : "Bajo";
        return new TraficoDemoResponse(zona, nivel, 0.3f + hash / 100f, "Demo (sustituir por API de tráfico en producción)");
    }

    private static ClimaResponse DemoClima(double lat, double lon) =>
        new(
            lat,
            lon,
            22.0,
            "Parcialmente nublado (demo sin OPENWEATHER_API_KEY)",
            "Demo local");
}

public record ApiInfo(string Nombre, string Proposito, string Config, string UsoEnProyecto);

public record ApiCatalogResponse(IReadOnlyList<ApiInfo> Apis);

public record ClimaResponse(double Lat, double Lon, double TemperaturaC, string Descripcion, string Fuente);

public record TraficoDemoResponse(string Zona, string Nivel, float FactorNormalizado, string Fuente);
