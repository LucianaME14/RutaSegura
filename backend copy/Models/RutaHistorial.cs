using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RutaSegura.Models
{
    public class RutaHistorial
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }

        [Required]
        [MaxLength(200)]
        public string OrigenTexto { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string DestinoTexto { get; set; } = string.Empty;

        /// <summary>peaton | bike</summary>
        [Required]
        [MaxLength(20)]
        public string Modo { get; set; } = "peaton";

        public int MinutosAprox { get; set; }

        public double KmAprox { get; set; }

        [MaxLength(120)]
        public string? RutaReferencia { get; set; }

        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}
