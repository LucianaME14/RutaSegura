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
    public class ContactosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContactosController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("usuario/{usuarioId:int}")]
        public async Task<IActionResult> GetByUsuario(int usuarioId)
        {
            var contactos = await _context.Contactos
                .Where(c => c.UsuarioId == usuarioId)
                .OrderBy(c => c.Prioridad)
                .ThenByDescending(c => c.EsPrincipal)
                .ToListAsync();

            return Ok(contactos);
        }

        [Authorize]
        [HttpGet("mios")]
        public async Task<IActionResult> GetMios()
        {
            var id = GetUserId();
            var contactos = await _context.Contactos
                .Where(c => c.UsuarioId == id)
                .OrderBy(c => c.Prioridad)
                .ThenByDescending(c => c.EsPrincipal)
                .ToListAsync();
            return Ok(contactos);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Contacto contacto)
        {
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == contacto.UsuarioId);
            if (!usuarioExiste)
            {
                return NotFound(new { message = "El usuario indicado no existe." });
            }

            contacto.Id = 0;
            contacto.CreadoEn = DateTime.UtcNow;
            _context.Contactos.Add(contacto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByUsuario), new { usuarioId = contacto.UsuarioId }, contacto);
        }

        [Authorize]
        [HttpPost("mios")]
        public async Task<IActionResult> CrearMio([FromBody] ContactoSolicitud body)
        {
            var c = new Contacto
            {
                UsuarioId = GetUserId(),
                Nombre = body.Nombre,
                Telefono = body.Telefono,
                Email = body.Email,
                Parentesco = body.Parentesco,
                Prioridad = body.Prioridad,
                EsPrincipal = body.EsPrincipal,
                CreadoEn = DateTime.UtcNow,
            };
            _context.Contactos.Add(c);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMios), c);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Contacto payload)
        {
            var contacto = await _context.Contactos.FindAsync(id);
            if (contacto is null)
            {
                return NotFound(new { message = "Contacto no encontrado." });
            }

            contacto.Nombre = payload.Nombre;
            contacto.Telefono = payload.Telefono;
            contacto.Email = payload.Email;
            contacto.Parentesco = payload.Parentesco;
            contacto.Prioridad = payload.Prioridad;
            contacto.EsPrincipal = payload.EsPrincipal;

            await _context.SaveChangesAsync();
            return Ok(contacto);
        }

        [Authorize]
        [HttpPut("mios/{id:int}")]
        public async Task<IActionResult> ActualizarMio(int id, [FromBody] ContactoSolicitud body)
        {
            var userId = GetUserId();
            var contacto = await _context.Contactos.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == userId);
            if (contacto is null) return NotFound();
            contacto.Nombre = body.Nombre;
            contacto.Telefono = body.Telefono;
            contacto.Email = body.Email;
            contacto.Parentesco = body.Parentesco;
            contacto.Prioridad = body.Prioridad;
            contacto.EsPrincipal = body.EsPrincipal;
            await _context.SaveChangesAsync();
            return Ok(contacto);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var contacto = await _context.Contactos.FindAsync(id);
            if (contacto is null)
            {
                return NotFound(new { message = "Contacto no encontrado." });
            }

            _context.Contactos.Remove(contacto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("mios/{id:int}")]
        public async Task<IActionResult> EliminarMio(int id)
        {
            var userId = GetUserId();
            var contacto = await _context.Contactos.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == userId);
            if (contacto is null) return NotFound();
            _context.Contactos.Remove(contacto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class ContactoSolicitud
        {
            public string Nombre { get; set; } = string.Empty;
            public string Telefono { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Parentesco { get; set; }
            public int Prioridad { get; set; } = 1;
            public bool EsPrincipal { get; set; }
        }
    }
}
