using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RutaSegura.Models
{
    public class Reporte
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo de incidente es requerido.")]
        public string TipoIncidente { get; set; }

        [Required]
        public string Ubicacion { get; set; }
        
        public string Latitud { get; set; }
        public string Longitud { get; set; }

        public string Descripcion { get; set; }
        public string UrlFotoEvidencia { get; set; }

        public DateTime FechaReporte { get; set; } = DateTime.UtcNow;

        public string Estado { get; set; } = "Pendiente"; // Pendiente, Aprobado, Rechazado

        // Relación con el usuario
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }
        
        // Campo para predicción IA
        public float NivelConfianzaIA { get; set; } 
    }
}