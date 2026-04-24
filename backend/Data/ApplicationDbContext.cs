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
        public DbSet<Contacto> Contactos { get; set; }
        public DbSet<Catalogo> Catalogos { get; set; }
        public DbSet<Sesion> Sesiones { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<UbicacionGuardada> UbicacionesGuardadas { get; set; }
        public DbSet<RutaHistorial> RutasHistorial { get; set; }
        public DbSet<ConfiguracionSistema> ConfiguracionSistema { get; set; }
        public DbSet<AlertaSistema> AlertasSistema { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Catalogo>()
                .HasIndex(c => new { c.Tipo, c.Codigo })
                .IsUnique();

            modelBuilder.Entity<Contacto>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Contactos)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reporte>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.Reportes)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reporte>()
                .HasOne(r => r.Catalogo)
                .WithMany(c => c.Reportes)
                .HasForeignKey(r => r.CatalogoId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Reporte>()
                .HasOne(r => r.Proyecto)
                .WithMany(p => p.Reportes)
                .HasForeignKey(r => r.ProyectoId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Sesion>()
                .HasOne(s => s.Usuario)
                .WithMany(u => u.Sesiones)
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Sesion>()
                .HasIndex(s => s.TokenJti)
                .IsUnique();

            modelBuilder.Entity<UbicacionGuardada>()
                .HasOne(u => u.Usuario)
                .WithMany(u => u.UbicacionesGuardadas)
                .HasForeignKey(u => u.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RutaHistorial>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.RutasHistorial)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RutaHistorial>()
                .HasIndex(r => new { r.UsuarioId, r.CreadoEn });

            modelBuilder.Entity<ConfiguracionSistema>(e => e.ToTable("ConfiguracionSistema"));
            modelBuilder.Entity<ConfiguracionSistema>().HasData(
                new ConfiguracionSistema
                {
                    Id = 1,
                    PesoZonasOscurasPct = 40,
                    CaducidadReporteMenorHoras = 24,
                    AutoAprobarConfianzaMinPct = 85,
                    PushNotificacionUrl = "https://push.rutasegura.net",
                });

            modelBuilder.Entity<AlertaSistema>(e => e.ToTable("AlertasSistema"));
        }
    }
}