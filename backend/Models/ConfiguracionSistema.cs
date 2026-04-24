using System.ComponentModel.DataAnnotations;

namespace RutaSegura.Models
{
    /// <summary>Fila única (Id = 1) con parámetros editables desde el panel admin.</summary>
    public class ConfiguracionSistema
    {
        [Key]
        public int Id { get; set; } = 1;

        [Range(0, 100)]
        public int PesoZonasOscurasPct { get; set; } = 40;

        [Range(1, 168)]
        public int CaducidadReporteMenorHoras { get; set; } = 24;

        [Range(50, 100)]
        public int AutoAprobarConfianzaMinPct { get; set; } = 85;

        [MaxLength(500)]
        public string? PushNotificacionUrl { get; set; } = "https://push.rutasegura.net";

        /// <summary>Opcional: clave almacenada en servidor (el front sigue usando VITE en el cliente).</summary>
        [MaxLength(2000)]
        public string? GoogleMapsKeyAlmacenada { get; set; }
    }
}
