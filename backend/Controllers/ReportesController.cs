using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RutaSegura.Models;
using RutaSegura.Data;
using System.Threading.Tasks;

namespace RutaSegura.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
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
                .Include(r => r.Usuario)
                .OrderByDescending(r => r.FechaReporte)
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
            
            // Lógica para enviar notificación a otros usuarios en la zona...
            
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Reporte aprobado correctamente." });
        }

        // POST: api/Reportes/Crear
        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] Reporte nuevoReporte)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría el llamado al servicio de IA para calcular NivelConfianzaIA
                nuevoReporte.FechaReporte = DateTime.UtcNow;
                _context.Reportes.Add(nuevoReporte);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Reporte creado exitosamente." });
            }
            return BadRequest(ModelState);
        }
    }
}