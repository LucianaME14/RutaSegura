using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RutaSegura.Models
{
    public class UbicacionGuardada
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }

        [Required]
        [MaxLength(120)]
        public string Etiqueta { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Direccion { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Latitud { get; set; }

        [MaxLength(50)]
        public string? Longitud { get; set; }

        [MaxLength(20)]
        public string? Icono { get; set; }

        public int Orden { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}
