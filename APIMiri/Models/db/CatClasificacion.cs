using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

/// <summary>
/// Catalogo de los clasificaciones
/// Se puede agregar nuevas clasificaciones solo el administrador
/// </summary>
public partial class CatClasificacion
{
    public int IdClasificacion { get; set; }

    public string? Clasificacion { get; set; }

    public virtual ICollection<ClasificacionTema> ClasificacionTemas { get; } = new List<ClasificacionTema>();
}
