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
        private readonly ILogger<ContactosController> _logger;

        public ContactosController(ApplicationDbContext context, ILogger<ContactosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return !string.IsNullOrEmpty(claim) && int.TryParse(claim, out userId);
        }

        private async Task<IActionResult?> RechazarSiUsuarioNoExiste(int userId)
        {
            if (await _context.Usuarios.AsNoTracking().AnyAsync(u => u.Id == userId))
                return null;

            return Unauthorized(new
            {
                message =
                    "Tu usuario ya no existe en el servidor (p. ej. tras un reinicio). Cierra sesión e inicia de nuevo.",
            });
        }

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
            if (!TryGetUserId(out var id))
                return Unauthorized(new { message = "Sesión inválida. Inicia sesión de nuevo." });
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
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { message = "Sesión inválida. Inicia sesión de nuevo." });

            var rechazo = await RechazarSiUsuarioNoExiste(userId);
            if (rechazo is not null) return rechazo;

            if (string.IsNullOrWhiteSpace(body.Nombre) || string.IsNullOrWhiteSpace(body.Telefono))
            {
                return BadRequest(new { message = "Nombre y teléfono son obligatorios." });
            }

            var c = new Contacto
            {
                UsuarioId = userId,
                Nombre = body.Nombre.Trim(),
                Telefono = body.Telefono.Trim(),
                Email = string.IsNullOrWhiteSpace(body.Email) ? null : body.Email.Trim(),
                Parentesco = string.IsNullOrWhiteSpace(body.Parentesco) ? null : body.Parentesco.Trim(),
                Prioridad = body.Prioridad < 1 ? 1 : body.Prioridad,
                EsPrincipal = body.EsPrincipal,
                CreadoEn = DateTime.UtcNow,
            };

            try
            {
                _context.Contactos.Add(c);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "No se pudo crear contacto para usuario {UserId}", userId);
                return StatusCode(500, new { message = "No se pudo guardar el contacto. Inicia sesión de nuevo." });
            }

            return StatusCode(StatusCodes.Status201Created, c);
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
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { message = "Sesión inválida. Inicia sesión de nuevo." });
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
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { message = "Sesión inválida. Inicia sesión de nuevo." });
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
