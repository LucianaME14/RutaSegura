using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RutaSegura.Data;

namespace RutaSegura.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CatalogoController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>GET /api/Catalogo — listado de ítems de catálogo (p. ej. tipos de incidente).</summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? tipo = "incidente", [FromQuery] bool soloActivos = true)
        {
            var q = _context.Catalogos.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(tipo))
            {
                q = q.Where(c => c.Tipo == tipo);
            }

            if (soloActivos)
            {
                q = q.Where(c => c.Activo);
            }

            var list = await q
                .OrderBy(c => c.Codigo)
                .Select(c => new
                {
                    c.Id,
                    c.Tipo,
                    c.Codigo,
                    c.Nombre,
                    c.Descripcion,
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}
