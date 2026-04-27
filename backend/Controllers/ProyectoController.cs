using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RutaSegura.Data;

namespace RutaSegura.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProyectoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProyectoController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>GET /api/Proyecto — proyectos registrados (Lab 2: entidad Proyecto en base de datos).</summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _context.Proyectos
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.Descripcion,
                    p.Estado,
                    p.FechaInicio,
                    p.FechaFin,
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}
