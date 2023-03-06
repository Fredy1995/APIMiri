using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

/// <summary>
/// Catálogo de tipos de archivos, referidos al contenido, no a su formato, por ejemplo:
/// Ley Ingresos
/// Oficios de entrega de claves catastrales
/// Proyectos de inversión
/// Capas de información cartográfica de tipos de suelo
/// </summary>
public partial class CatTipoArchivo
{
    public int IdTipoArchivo { get; set; }

    public int TipoArchivo { get; set; }

    public int IdTablaAdjunta { get; set; }

    public int IdExtension { get; set; }

    public virtual ICollection<Archivo> Archivos { get; } = new List<Archivo>();

    public virtual ICollection<CatMetadatoXarchivo> CatMetadatoXarchivos { get; } = new List<CatMetadatoXarchivo>();

    public virtual CatExtensione IdExtensionNavigation { get; set; } = null!;

    public virtual TablaAdjuntum IdTablaAdjuntaNavigation { get; set; } = null!;
}
