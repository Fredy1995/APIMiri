using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

/// <summary>
/// Indica quienes son los responsables de cada tema, tiene la capacidad de crear nuevas clasificaciones del Tema
/// </summary>
public partial class TemaUsuario
{
    public int IdTemaUsuario { get; set; }

    public int IdTema { get; set; }

    public int IdUsuario { get; set; }

    public virtual CatTema IdTemaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
