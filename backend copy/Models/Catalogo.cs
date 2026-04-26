using System.ComponentModel.DataAnnotations;

namespace RutaSegura.Models
{
    public class Catalogo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Tipo { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime? ActualizadoEn { get; set; }

        public virtual ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
    }
}
