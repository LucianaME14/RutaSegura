using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RutaSegura.Data;
using RutaSegura.Models;

namespace RutaSegura.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private const int MaxEvidenciaLength = 1_200_000;
        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Reportes
        [HttpGet]
        public async Task<IActionResult> GetReportes()
        {
            var reportes = await _context.Reportes
                .AsNoTracking()
                .OrderByDescending(r => r.FechaReporte)
                .Select(
                    r => new
                    {
                        r.Id,
                        r.TipoIncidente,
                        r.Ubicacion,
                        r.Descripcion,
                        r.Estado,
                        r.FechaReporte,
                        r.NivelConfianzaIA,
                        r.Latitud,
                        r.Longitud,
                        r.EsAnonimo,
                        Usuario = r.Usuario == null
                            ? null
                            : new { r.Usuario.Nombre, r.Usuario.Email },
                    })
                .ToListAsync();

            return Ok(reportes);
        }

        /// <summary>Últimos reportes para inicio (sin datos sensibles). Incluye pendientes y aprobados.</summary>
        [AllowAnonymous]
        [HttpGet("recientes")]
        public async Task<IActionResult> GetRecientes(
            [FromQuery] int take = 8,
            [FromQuery] int maxDays = 30)
        {
            var n = Math.Clamp(take, 1, 30);
            var from = DateTime.UtcNow.AddDays(-Math.Clamp(maxDays, 1, 365));
            var list = await _context.Reportes
                .AsNoTracking()
                .Where(r =>
                    r.FechaReporte >= from
                    && r.Estado != "Rechazado")
                .OrderByDescending(r => r.FechaReporte)
                .Take(n)
                .Select(r => new
                {
                    r.Id,
                    r.TipoIncidente,
                    r.Ubicacion,
                    r.Descripcion,
                    r.FechaReporte,
                })
                .ToListAsync();
            return Ok(list);
        }

        [Authorize]
        [HttpGet("mios")]
        public async Task<IActionResult> GetMios()
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var reportes = await _context.Reportes
                .Where(r => r.UsuarioId == id)
                .OrderByDescending(r => r.FechaReporte)
                .Select(r => new
                {
                    r.Id,
                    r.TipoIncidente,
                    r.Ubicacion,
                    r.Estado,
                    r.FechaReporte,
                    r.EsAnonimo,
                    r.Descripcion,
                })
                .ToListAsync();
            return Ok(reportes);
        }

        // POST: api/Reportes/Aprobar/5
        [HttpPost("Aprobar/{id}")]
        public async Task<IActionResult> Aprobar(int id)
        {
            var reporte = await _context.Reportes.FindAsync(id);
            if (reporte == null) return NotFound();

            reporte.Estado = "Aprobado";
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Reporte aprobado correctamente." });
        }

        [HttpPost("Rechazar/{id}")]
        public async Task<IActionResult> Rechazar(int id)
        {
            var reporte = await _context.Reportes.FindAsync(id);
            if (reporte == null) return NotFound();

            reporte.Estado = "Rechazado";
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Reporte rechazado." });
        }

        public class CrearReporteRequest
        {
            [Required]
            public string TipoIncidente { get; set; } = string.Empty;

            [Required]
            public string Ubicacion { get; set; } = string.Empty;

            public string? Descripcion { get; set; }
            public string? Latitud { get; set; }
            public string? Longitud { get; set; }
            public string? UrlFotoEvidencia { get; set; }
            public bool EsAnonimo { get; set; }
        }

        [Authorize]
        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] CrearReporteRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (!string.IsNullOrEmpty(req.UrlFotoEvidencia) &&
                req.UrlFotoEvidencia.Length > MaxEvidenciaLength)
            {
                return BadRequest(
                    new { message = "La evidencia (foto, PDF o Word) es demasiado grande. Reduce el tamaño del archivo." });
            }

            var reporte = new Reporte
            {
                TipoIncidente = req.TipoIncidente,
                Ubicacion = req.Ubicacion,
                Descripcion = req.Descripcion,
                Latitud = req.Latitud,
                Longitud = req.Longitud,
                UrlFotoEvidencia = req.UrlFotoEvidencia,
                EsAnonimo = req.EsAnonimo,
                UsuarioId = userId,
                FechaReporte = DateTime.UtcNow,
                Estado = "Pendiente",
                NivelConfianzaIA = 0.75f + (float)(Random.Shared.NextDouble() * 0.2),
            };

            _context.Reportes.Add(reporte);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Reporte creado exitosamente.", id = reporte.Id });
        }
    }
}
