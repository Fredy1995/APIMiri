using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

/// <summary>
/// Contiene  los responsables del contenido de las terceras ramas
/// El campo permiso indica
/// * 0 puede leer y descargar
/// * 1 puede subir contenido + 0
/// * 2 Puede borrar + 1
/// </summary>
public partial class UsuariosGct
{
    public int IdUgct { get; set; }

    public int IdUsuario { get; set; }

    public int Permiso { get; set; }

    public int IdGct { get; set; }

    public virtual GrupoClasificacionTema IdGctNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
