using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

public partial class CatPerfile
{
    public int IdPerfil { get; set; }

    public string Perfil { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; } = new List<Usuario>();
}
