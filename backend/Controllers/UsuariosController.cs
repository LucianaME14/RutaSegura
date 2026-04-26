using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RutaSegura.Data;
using RutaSegura.Models;
using RutaSegura.Services;

namespace RutaSegura.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RedisService _redis;

        public UsuariosController(ApplicationDbContext context, RedisService redis)
        {
            _context = context;
            _redis = redis;
        }

        // =========================
        // GET: api/Usuarios
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var cacheKey = "usuarios:todos";

            if (_redis.IsEnabled)
            {
                var cache = await _redis.GetStringAsync(cacheKey);
                if (cache != null)
                    return Ok(JsonSerializer.Deserialize<object>(cache));
            }

            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .Select(u => new
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

            if (_redis.IsEnabled)
            {
                await _redis.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(usuarios),
                    TimeSpan.FromMinutes(5)
                );
            }

            return Ok(usuarios);
        }

        // =========================
        // GET: api/Usuarios/{id}/resumen
        // =========================
        [HttpGet("{id:int}/resumen")]
        public async Task<IActionResult> GetResumen(int id)
        {
            var cacheKey = $"usuarios:resumen:{id}";

            if (_redis.IsEnabled)
            {
                var cache = await _redis.GetStringAsync(cacheKey);
                if (cache != null)
                    return Ok(JsonSerializer.Deserialize<object>(cache));
            }

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new
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

            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            var ultimos = await _context.Reportes
                .AsNoTracking()
                .Where(r => r.UsuarioId == id)
                .OrderByDescending(r => r.FechaReporte)
                .Take(8)
                .Select(r => new
                {
                    r.Id,
                    r.TipoIncidente,
                    r.Ubicacion,
                    r.Estado,
                    r.FechaReporte,
                })
                .ToListAsync();

            var resultado = new
            {
                usuario,
                ultimosReportes = ultimos
            };

            if (_redis.IsEnabled)
            {
                await _redis.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(resultado),
                    TimeSpan.FromMinutes(5)
                );
            }

            return Ok(resultado);
        }

        // =========================
        // PUT: actualizar usuario
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, Usuario updated)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.Nombre = updated.Nombre;
            usuario.Telefono = updated.Telefono;
            usuario.Estado = updated.Estado;
            usuario.Rol = updated.Rol;

            await _context.SaveChangesAsync();

            await LimpiarCacheUsuarios(id);

            return Ok(new { message = "Usuario actualizado correctamente" });
        }

        // =========================
        // PATCH: activar/desactivar
        // =========================
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.Estado = usuario.Estado == "Activo" ? "Inactivo" : "Activo";

            await _context.SaveChangesAsync();

            await LimpiarCacheUsuarios(id);

            return Ok(new { message = "Estado actualizado", estado = usuario.Estado });
        }

        // =========================
        // 🔥 LIMPIAR CACHE
        // =========================
        private async Task LimpiarCacheUsuarios(int usuarioId)
        {
            if (!_redis.IsEnabled) return;

            await _redis.RemoveAsync("usuarios:todos");
            await _redis.RemoveAsync($"usuarios:resumen:{usuarioId}");
        }
    }
}