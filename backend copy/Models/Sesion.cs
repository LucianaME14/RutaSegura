using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RutaSegura.Models
{
    public class Sesion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }

        [Required]
        [MaxLength(100)]
        public string TokenJti { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? RefreshTokenHash { get; set; }

        [MaxLength(100)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        [MaxLength(30)]
        public string Origen { get; set; } = "web";

        public DateTime CreadaEn { get; set; } = DateTime.UtcNow;
        public DateTime ExpiraEn { get; set; }
        public DateTime? CerradaEn { get; set; }

        [MaxLength(30)]
        public string Estado { get; set; } = "Activa";
    }
}
