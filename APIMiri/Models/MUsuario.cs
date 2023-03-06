namespace APIMiri.Models
{
    public class MUsuario
    {
        public int IdUsuario { get; set; }

        public string? Usuario1 { get; set; }

        public string? Contraseña { get; set; }

        public string? Nombre { get; set; }

        public string? APaterno { get; set; }

        public string? AMaterno { get; set; }

        public int? IdPerfil { get; set; }

        public string? Perfil { get; set; }

        public DateTime? FechaIngreso { get; set; }

        public bool? check { get; set; }
    }
}
