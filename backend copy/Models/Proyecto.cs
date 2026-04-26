using System.ComponentModel.DataAnnotations;

namespace RutaSegura.Models
{
    public class Proyecto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [MaxLength(30)]
        public string Estado { get; set; } = "Activo";

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
    }
}
