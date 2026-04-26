using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RutaSegura.Data;
using RutaSegura.Services;

namespace RutaSegura.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RedisService _redisService;

        public SessionController(ApplicationDbContext context, RedisService redisService)
        {
            _context = context;
            _redisService = redisService;
        }

        [HttpGet("usuario/{usuarioId:int}")]
        public async Task<IActionResult> ListarPorUsuario(int usuarioId)
        {
            var sesiones = await _context.Sesiones
                .Where(s => s.UsuarioId == usuarioId)
                .OrderByDescending(s => s.CreadaEn)
                .Select(s => new
                {
                    s.Id,
                    s.Origen,
                    s.Estado,
                    s.CreadaEn,
                    s.ExpiraEn,
                    s.CerradaEn,
                    s.IpAddress,
                    s.UserAgent,
                })
                .ToListAsync();

            return Ok(sesiones);
        }

        [HttpPost("revocar/{id:int}")]
        public async Task<IActionResult> Revocar(int id)
        {
            var sesion = await _context.Sesiones.FindAsync(id);
            if (sesion is null)
            {
                return NotFound(new { message = "Sesión no encontrada." });
            }

            sesion.Estado = "Revocada";
            sesion.CerradaEn = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _redisService.RemoveAsync($"sesion:{sesion.TokenJti}");
            return Ok(new { message = "Sesión revocada correctamente." });
        }
    }
}
