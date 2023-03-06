using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

/// <summary>
/// Catálogo de los campos a través de los cuales se buscará, dependiendo del tipo de archivo.
/// Cada campo tendrá una expresión regular que indica el patrón de la información que se busca.
/// </summary>
public partial class CatMetadatoXarchivo
{
    public int IdMetadato { get; set; }

    public int IdTipoArchivo { get; set; }

    public string MetaDato { get; set; } = null!;

    public string ExpresionRegular { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public virtual CatTipoArchivo IdTipoArchivoNavigation { get; set; } = null!;

    public virtual ICollection<MetadatoPorArchivo> MetadatoPorArchivos { get; } = new List<MetadatoPorArchivo>();
}
