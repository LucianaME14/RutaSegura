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
    [Authorize]
    public class UbicacionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UbicacionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var s = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(s!);
        }

        [HttpGet("mias")]
        public async Task<IActionResult> GetMias()
        {
            var id = GetUserId();
            var list = await _context.UbicacionesGuardadas
                .Where(u => u.UsuarioId == id)
                .OrderBy(u => u.Orden)
                .ThenBy(u => u.Etiqueta)
                .ToListAsync();
            return Ok(list);
        }

        [HttpPost("mias")]
        public async Task<IActionResult> Crear([FromBody] UbicacionGuardada body)
        {
            body.Id = 0;
            body.UsuarioId = GetUserId();
            body.CreadoEn = DateTime.UtcNow;
            _context.UbicacionesGuardadas.Add(body);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMias), body);
        }

        [HttpPut("mias/{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] UbicacionGuardada payload)
        {
            var userId = GetUserId();
            var item = await _context.UbicacionesGuardadas
                .FirstOrDefaultAsync(u => u.Id == id && u.UsuarioId == userId);
            if (item is null) return NotFound();
            item.Etiqueta = payload.Etiqueta;
            item.Direccion = payload.Direccion;
            item.Latitud = payload.Latitud;
            item.Longitud = payload.Longitud;
            item.Icono = payload.Icono;
            item.Orden = payload.Orden;
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        [HttpDelete("mias/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var userId = GetUserId();
            var item = await _context.UbicacionesGuardadas
                .FirstOrDefaultAsync(u => u.Id == id && u.UsuarioId == userId);
            if (item is null) return NotFound();
            _context.UbicacionesGuardadas.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
