using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

/// <summary>
/// Contiene todos los Temas y sus clasificaciones.
/// (Segunda Rama)
/// Debe existir como mínimo una clasificación por cada Tema
/// Los responsables de los temas pueden crear nuevas clasificaciones
/// 
/// Al crear una nueva segunda rama Tema-Clasificacion se deberá:
/// * Crear un nuevo grupo (tercera rama)  que pertenezca a la segunda Rama.
/// * A las ramas creadas (segunda y tercera) se pondrá como responsable al creador.
/// </summary>
public partial class ClasificacionTema
{
    public int IdCt { get; set; }

    public int IdTema { get; set; }

    public int IdClasificacion { get; set; }

    public virtual ICollection<GrupoClasificacionTema> GrupoClasificacionTemas { get; } = new List<GrupoClasificacionTema>();

    public virtual CatClasificacion IdClasificacionNavigation { get; set; } = null!;

    public virtual CatTema IdTemaNavigation { get; set; } = null!;

    public virtual ICollection<UsuariosCt> UsuariosCts { get; } = new List<UsuariosCt>();
}
