using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

/// <summary>
/// Contiene  los responsables de las segundas ramas
/// Pueden crear terceras ramas
/// Puede agregar responsables a esta segunda rama
/// </summary>
public partial class UsuariosCt
{
    public int IdClasifUsuario { get; set; }

    public int IdUsuario { get; set; }

    public int IdCt { get; set; }

    public virtual ClasificacionTema IdCtNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
