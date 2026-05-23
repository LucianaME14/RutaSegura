using Microsoft.AspNetCore.Mvc;
using RutaSegura.Services;

namespace RutaSegura.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExternalController : ControllerBase
{
    private readonly ExternalApisService _external;

    public ExternalController(ExternalApisService external) => _external = external;

    /// <summary>Catálogo de APIs externas integradas o documentadas.</summary>
    [HttpGet("catalogo")]
    public IActionResult Catalogo() => Ok(_external.GetCatalog());

    /// <summary>Clima actual (OpenWeather o demo).</summary>
    [HttpGet("clima")]
    public async Task<IActionResult> Clima(
        [FromQuery] double lat = -12.0464,
        [FromQuery] double lon = -77.0428,
        CancellationToken ct = default) =>
        Ok(await _external.GetClimaAsync(lat, lon, ct));

    /// <summary>Tráfico estimado demo para enriquecer variables ML.</summary>
    [HttpGet("trafico-demo")]
    public IActionResult TraficoDemo([FromQuery] string zona = "Lima") =>
        Ok(_external.GetTraficoDemo(zona));
}
