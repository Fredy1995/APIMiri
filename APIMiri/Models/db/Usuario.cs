using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

/// <summary>
/// Usuarios de MIRI
/// el numUsuario es rescatado de GUSIT
/// </summary>
public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string? Usuario1 { get; set; }

    public byte[]? Contraseña { get; set; }

    public string? Nombre { get; set; }

    public string? APaterno { get; set; }

    public string? AMaterno { get; set; }

    public int? IdPerfil { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public bool? Habilitado { get; set; }

    public virtual CatPerfile? IdPerfilNavigation { get; set; }

    public virtual ICollection<TemaUsuario> TemaUsuarios { get; } = new List<TemaUsuario>();

    public virtual ICollection<UsuariosCt> UsuariosCts { get; } = new List<UsuariosCt>();

    public virtual ICollection<UsuariosGct> UsuariosGcts { get; } = new List<UsuariosGct>();
}
