using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

/// <summary>
/// Catalogo de grupo
/// Si se pueden agregar nuevos grupos solo el administrador
/// </summary>
public partial class CatGrupo
{
    public int IdGrupo { get; set; }

    public string Grupo { get; set; } = null!;

    public int IdTipoArchivo { get; set; }

    public string? Propietario { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public string? ModificadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual ICollection<GrupoClasificacionTema> GrupoClasificacionTemas { get; } = new List<GrupoClasificacionTema>();
}
