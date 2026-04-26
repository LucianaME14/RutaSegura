using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RutaSegura.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        public string Rol { get; set; } = "Usuario";

        public string Estado { get; set; } = "Activo";

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Relación con reportes
        public virtual ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
        public virtual ICollection<Contacto> Contactos { get; set; } = new List<Contacto>();
        public virtual ICollection<Sesion> Sesiones { get; set; } = new List<Sesion>();
        public virtual ICollection<UbicacionGuardada> UbicacionesGuardadas { get; set; } =
            new List<UbicacionGuardada>();
        public virtual ICollection<RutaHistorial> RutasHistorial { get; set; } = new List<RutaHistorial>();
    }
}