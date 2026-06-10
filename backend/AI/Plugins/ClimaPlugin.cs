using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel;
using RutaSegura.Services;

namespace RutaSegura.AI.Plugins;

/// <summary>Plugin Semantic Kernel: clima actual vía WeatherAPI (con caché Redis).</summary>
public class ClimaPlugin
{
    private readonly ExternalApisService _external;

    public ClimaPlugin(ExternalApisService external) => _external = external;

    [KernelFunction("obtener_clima_actual")]
    [Description("Obtiene el clima actual para coordenadas (Lima por defecto).")]
    public async Task<string> ObtenerClimaActualAsync(
        double lat = -12.0464,
        double lon = -77.0428)
    {
        var clima = await _external.GetClimaAsync(lat, lon);
        return JsonSerializer.Serialize(new
        {
            clima.Lat,
            clima.Lon,
            clima.TemperaturaC,
            clima.Descripcion,
            clima.Fuente,
            clima.PrecipMm,
            clima.VisKm,
            impacto_movilidad = clima.TemperaturaC < 10 || clima.Descripcion.Contains("lluv", StringComparison.OrdinalIgnoreCase)
                ? "Condiciones que pueden afectar visibilidad o movilidad peatonal."
                : "Condiciones favorables para desplazamiento.",
        });
    }
}
