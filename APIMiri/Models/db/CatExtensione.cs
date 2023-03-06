using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

public partial class CatExtensione
{
    public int IdExtension { get; set; }

    public string? Extension { get; set; }

    public string? NombreExtension { get; set; }

    public virtual ICollection<CatTipoArchivo> CatTipoArchivos { get; } = new List<CatTipoArchivo>();
}
