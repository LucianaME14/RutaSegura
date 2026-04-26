using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RutaSegura.Data;
using RutaSegura.Models;

namespace RutaSegura.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .Select(
                    u => new
                    {
                        u.Id,
                        u.Nombre,
                        u.Email,
                        u.Telefono,
                        u.Rol,
                        u.Estado,
                        u.FechaRegistro,
                        reportesCreados = u.Reportes.Count,
                    })
                .ToListAsync();

            return Ok(usuarios);
        }

        /// <summary>Detalle para panel admin: datos de usuario e incidencias recientes.</summary>
        [HttpGet("{id:int}/resumen")]
        public async Task<IActionResult> GetResumen(int id)
        {
            var u = await _context.Usuarios
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(
                    x => new
                    {
                        x.Id,
                        x.Nombre,
                        x.Email,
                        x.Telefono,
                        x.Rol,
                        x.Estado,
                        x.FechaRegistro,
                        reportesCreados = x.Reportes.Count,
                    })
                .FirstOrDefaultAsync();

            if (u == null) return NotFound(new { message = "Usuario no encontrado." });

            var ultimos = await _context.Reportes
                .AsNoTracking()
                .Where(r => r.UsuarioId == id)
                .OrderByDescending(r => r.FechaReporte)
                .Take(8)
                .Select(
                    r => new
                    {
                        r.Id,
                        r.TipoIncidente,
                        r.Ubicacion,
                        r.Estado,
                        r.FechaReporte,
                    })
                .ToListAsync();

            return Ok(new { usuario = u, ultimosReportes = ultimos });
        }
    }
}
