using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

/// <summary>
/// Catalogo de temas
/// (Primera Rama)
/// Fijos no se pueden agregar mas registros!!!
/// </summary>
public partial class CatTema
{
    public int IdTema { get; set; }

    public string Tema { get; set; } = null!;

    public string? Propietario { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public string? ModificadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual ICollection<ClasificacionTema> ClasificacionTemas { get; } = new List<ClasificacionTema>();

    public virtual ICollection<TemaUsuario> TemaUsuarios { get; } = new List<TemaUsuario>();
}
