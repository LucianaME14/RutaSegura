using Microsoft.EntityFrameworkCore;
using RutaSegura.Models;

namespace RutaSegura.Data
{
    public static class DbSeeder
    {
        /// <summary>Datos mínimos de Lab 2: un proyecto y catálogo de tipos de incidente (códigos alineados con el front de Reportar).</summary>
        public static async Task SeedCatalogoYProyectoAsync(ApplicationDbContext db)
        {
            if (!await db.Proyectos.AnyAsync(p => p.Nombre == "RutaSegura"))
            {
                db.Proyectos.Add(
                    new Proyecto
                    {
                        Nombre = "RutaSegura",
                        Descripcion = "Aplicación web y API — requisito Proyecto (Lab 2).",
                        Estado = "Activo",
                        FechaInicio = DateTime.UtcNow,
                    });
                await db.SaveChangesAsync();
            }

            var codigos = new (string Codigo, string Nombre, string? Desc)[]
            {
                ("robo", "Robo", null),
                ("acoso", "Acoso", null),
                ("luz", "Sin iluminación", null),
                ("hueco", "Hueco en vía", null),
                ("accidente", "Accidente", null),
                ("otro", "Otro peligro", null),
            };

            foreach (var row in codigos)
            {
                var exists = await db.Catalogos.AnyAsync(c =>
                    c.Tipo == "incidente" && c.Codigo == row.Codigo);
                if (exists) continue;

                db.Catalogos.Add(
                    new Catalogo
                    {
                        Tipo = "incidente",
                        Codigo = row.Codigo,
                        Nombre = row.Nombre,
                        Descripcion = row.Desc,
                        Activo = true,
                    });
            }

            if (db.ChangeTracker.HasChanges())
            {
                await db.SaveChangesAsync();
            }
        }
    }
}
