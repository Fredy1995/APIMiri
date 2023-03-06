using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

/// <summary>
/// Contiene los campos a buscar de la tabla adjunta por tipo de documento
/// </summary>
public partial class TablaAdjuntum
{
    public int IdTablaAdjunta { get; set; }

    public string NombreCampo { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public string? NombreTabla { get; set; }

    public virtual ICollection<CatTipoArchivo> CatTipoArchivos { get; } = new List<CatTipoArchivo>();
}
