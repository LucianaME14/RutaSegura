using System.ComponentModel.DataAnnotations;
/// cambio
namespace RutaSegura.Models 
{
    public class AlertaSistema
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Detalle { get; set; }

        /// <summary>alta | media | baja</summary>
        [Required]
        [MaxLength(20)]
        public string Prioridad { get; set; } = "media";

        /// <summary>Auto = generada desde predicción; Manual = creada por admin (futuro)</summary>
        [Required]
        [MaxLength(20)]
        public string Origen { get; set; } = "Auto";

        [MaxLength(500)]
        public string? UbicacionRef { get; set; }

        public int RiesgoEstimadoPct { get; set; }

        public DateTime CreadaEn { get; set; } = DateTime.UtcNow;
    }
}
