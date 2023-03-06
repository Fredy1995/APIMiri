using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

/// <summary>
/// Contiene la ubicación física/logica de los archivos digitales subidos al servidor de archivos. Indicando el tipo de archivo que es y una breve descripción.
/// </summary>
public partial class Archivo
{
    public int IdArchivo { get; set; }

    public string NombreArchivo { get; set; } = null!;

    public string RutaArchivo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int IdTipoArchivo { get; set; }

    public int? IdGct { get; set; }

    public int IdUsuario { get; set; }

    public virtual GrupoClasificacionTema? IdGctNavigation { get; set; }

    public virtual CatTipoArchivo IdTipoArchivoNavigation { get; set; } = null!;

    public virtual ICollection<MetadatoPorArchivo> MetadatoPorArchivos { get; } = new List<MetadatoPorArchivo>();
}
