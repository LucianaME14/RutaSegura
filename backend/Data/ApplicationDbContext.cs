using Microsoft.EntityFrameworkCore;
using RutaSegura.Models;

namespace RutaSegura.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Reporte> Reportes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}