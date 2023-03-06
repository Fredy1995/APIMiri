using System;
using System.Collections.Generic;

namespace APIMiri.Models.db;

/// <summary>
/// Contiene todos los Temas y sus clasificaciones.
/// (Segunda Rama)
/// Debe existir como mínimo una clasificación por cada Tema
/// Los responsables de las segundas ramas pueden crear nuevos grupos para esa rama.
/// 
/// Al crear una nueva tercera rama Tema-Clasificacion-Grupo se pondrá como responsable al creador.
/// </summary>
public partial class GrupoClasificacionTema
{
    public int IdGct { get; set; }

    public int IdCt { get; set; }

    public int IdGrupo { get; set; }

    public virtual ICollection<Archivo> Archivos { get; } = new List<Archivo>();

    public virtual ClasificacionTema IdCtNavigation { get; set; } = null!;

    public virtual CatGrupo IdGrupoNavigation { get; set; } = null!;

    public virtual ICollection<UsuariosGct> UsuariosGcts { get; } = new List<UsuariosGct>();
}
