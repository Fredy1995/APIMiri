namespace APIMiri.Models
{
    public class MDetallesDirectorio
    {
        public string Directorio { get; set; } = null!;

        public string? Propietario { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public string? ModificadoPor { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}
