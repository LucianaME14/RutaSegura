using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace RutaSegura.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeoController : ControllerBase
    {
        private static readonly TimeSpan NominatimTimeout = TimeSpan.FromSeconds(8);
        private readonly IHttpClientFactory _httpClientFactory;

        public GeoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Convierte lat/lon en una dirección legible (OpenStreetMap Nominatim, uso razonable).
        /// </summary>
        [HttpGet("reverse")]
        [Produces("application/json")]
        public async Task<IActionResult> Reverse([FromQuery] string? lat, [FromQuery] string? lon, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(lat) || string.IsNullOrWhiteSpace(lon))
            {
                return BadRequest(new { message = "Parámetros lat y lon requeridos." });
            }

            if (!double.TryParse(
                    lat,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var la) ||
                !double.TryParse(
                    lon,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var lo))
            {
                return BadRequest(new { message = "Coordenadas no válidas." });
            }

            if (la < -90 || la > 90 || lo < -180 || lo > 180)
            {
                return BadRequest(new { message = "Coordenadas fuera de rango." });
            }

            var client = _httpClientFactory.CreateClient();
            client.Timeout = NominatimTimeout;
            client.DefaultRequestHeaders.TryAddWithoutValidation(
                "User-Agent",
                "RutaSegura/1.0 (mapa; contacto: desarrollo local)");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "es,es-PE,en");

            var laStr = la.ToString(CultureInfo.InvariantCulture);
            var loStr = lo.ToString(CultureInfo.InvariantCulture);
            var url = $"https://nominatim.openstreetmap.org/reverse?lat={laStr}&lon={loStr}&format=json";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage resp;
            try
            {
                resp = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            }
            catch (Exception)
            {
                return Ok(new { address = (string?)null, coordinates = new { lat = laStr, lon = loStr } });
            }

            if (!resp.IsSuccessStatusCode)
            {
                return Ok(new { address = (string?)null, coordinates = new { lat = laStr, lon = loStr } });
            }

            NominatimReverse? data;
            try
            {
                data = await resp.Content.ReadFromJsonAsync<NominatimReverse>(cancellationToken: ct);
            }
            catch
            {
                return Ok(new { address = (string?)null, coordinates = new { lat = laStr, lon = loStr } });
            }

            return Ok(new
            {
                address = data?.DisplayName,
                coordinates = new { lat = laStr, lon = loStr },
            });
        }

        private class NominatimReverse
        {
            [JsonPropertyName("display_name")]
            public string? DisplayName { get; set; }
        }
    }
}
