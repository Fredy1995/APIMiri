using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

/// <summary>
/// Contiene los valores de los metadatos de cada archivo digital subido al servidor de aechivos.
/// </summary>
public partial class MetadatoPorArchivo
{
    public int IdmetadatoArchivo { get; set; }

    public int IdArchivo { get; set; }

    public int IdMetadato { get; set; }

    public string? ValorMetadato { get; set; }

    public virtual Archivo IdArchivoNavigation { get; set; } = null!;

    public virtual CatMetadatoXarchivo IdMetadatoNavigation { get; set; } = null!;
}
