using System;
using System.Collections.Generic;
using APIMiri.Models.db;
using Microsoft.EntityFrameworkCore;

namespace APIMiri.Data;

public partial class DbMiriContext : DbContext
{
    public DbMiriContext()
    {
    }

    public DbMiriContext(DbContextOptions<DbMiriContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Archivo> Archivos { get; set; }

    public virtual DbSet<CatClasificacion> CatClasificacions { get; set; }

    public virtual DbSet<CatExtensione> CatExtensiones { get; set; }

    public virtual DbSet<CatGrupo> CatGrupos { get; set; }

    public virtual DbSet<CatMetadatoXarchivo> CatMetadatoXarchivos { get; set; }

    public virtual DbSet<CatPerfile> CatPerfiles { get; set; }

    public virtual DbSet<CatTema> CatTemas { get; set; }

    public virtual DbSet<CatTipoArchivo> CatTipoArchivos { get; set; }

    public virtual DbSet<ClasificacionTema> ClasificacionTemas { get; set; }

    public virtual DbSet<GrupoClasificacionTema> GrupoClasificacionTemas { get; set; }

    public virtual DbSet<MetadatoPorArchivo> MetadatoPorArchivos { get; set; }

    public virtual DbSet<TablaAdjuntum> TablaAdjunta { get; set; }

    public virtual DbSet<TemaUsuario> TemaUsuarios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuariosCt> UsuariosCts { get; set; }

    public virtual DbSet<UsuariosGct> UsuariosGcts { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=192.168.12.11; Database=dbMIRI;User Id=usMIRI; password=u5M1r1*;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Archivo>(entity =>
        {
            entity.HasKey(e => e.IdArchivo);

            entity.ToTable(tb => tb.HasComment("Contiene la ubicación física/logica de los archivos digitales subidos al servidor de archivos. Indicando el tipo de archivo que es y una breve descripción."));

            entity.Property(e => e.IdArchivo).HasColumnName("idArchivo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.IdGct).HasColumnName("idGCT");
            entity.Property(e => e.IdTipoArchivo).HasColumnName("idTipoArchivo");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.NombreArchivo)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombreArchivo");
            entity.Property(e => e.RutaArchivo)
                .HasMaxLength(350)
                .IsUnicode(false)
                .HasColumnName("rutaArchivo");

            entity.HasOne(d => d.IdGctNavigation).WithMany(p => p.Archivos)
                .HasForeignKey(d => d.IdGct)
                .HasConstraintName("FK_Archivos_GrupoClasificacionTema");

            entity.HasOne(d => d.IdTipoArchivoNavigation).WithMany(p => p.Archivos)
                .HasForeignKey(d => d.IdTipoArchivo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Archivos_CatTipoArchivo");
        });

        modelBuilder.Entity<CatClasificacion>(entity =>
        {
            entity.HasKey(e => e.IdClasificacion);

            entity.ToTable("CatClasificacion", tb => tb.HasComment("Catalogo de los clasificaciones\r\nSe puede agregar nuevas clasificaciones solo el administrador"));

            entity.Property(e => e.IdClasificacion).HasColumnName("idClasificacion");
            entity.Property(e => e.Clasificacion)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("clasificacion");
        });

        modelBuilder.Entity<CatExtensione>(entity =>
        {
            entity.HasKey(e => e.IdExtension);

            entity.Property(e => e.IdExtension).HasColumnName("idExtension");
            entity.Property(e => e.Extension)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NombreExtension)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("nombreExtension");
        });

        modelBuilder.Entity<CatGrupo>(entity =>
        {
            entity.HasKey(e => e.IdGrupo);

            entity.ToTable("CatGrupo", tb => tb.HasComment("Catalogo de grupo\r\nSi se pueden agregar nuevos grupos solo el administrador"));

            entity.Property(e => e.IdGrupo).HasColumnName("idGrupo");
            entity.Property(e => e.Grupo)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("grupo");
            entity.Property(e => e.IdTipoArchivo).HasColumnName("idTipoArchivo");
        });

        modelBuilder.Entity<CatMetadatoXarchivo>(entity =>
        {
            entity.HasKey(e => e.IdMetadato).HasName("PK_CatMetadato");

            entity.ToTable("CatMetadatoXArchivo", tb => tb.HasComment("Catálogo de los campos a través de los cuales se buscará, dependiendo del tipo de archivo.\r\nCada campo tendrá una expresión regular que indica el patrón de la información que se busca."));

            entity.Property(e => e.IdMetadato).HasColumnName("idMetadato");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(350)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.ExpresionRegular)
                .HasMaxLength(450)
                .IsUnicode(false)
                .HasColumnName("expresionRegular");
            entity.Property(e => e.IdTipoArchivo).HasColumnName("idTipoArchivo");
            entity.Property(e => e.MetaDato)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("metaDato");

            entity.HasOne(d => d.IdTipoArchivoNavigation).WithMany(p => p.CatMetadatoXarchivos)
                .HasForeignKey(d => d.IdTipoArchivo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CatMetadato_CatTipoArchivo");
        });

        modelBuilder.Entity<CatPerfile>(entity =>
        {
            entity.HasKey(e => e.IdPerfil);

            entity.Property(e => e.IdPerfil).HasColumnName("idPerfil");
            entity.Property(e => e.Perfil)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("perfil");
        });

        modelBuilder.Entity<CatTema>(entity =>
        {
            entity.HasKey(e => e.IdTema);

            entity.ToTable("CatTema", tb => tb.HasComment("Catalogo de temas\r\n(Primera Rama)\r\nFijos no se pueden agregar mas registros!!!"));

            entity.Property(e => e.IdTema).HasColumnName("idTema");
            entity.Property(e => e.Tema)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("tema");
        });

        modelBuilder.Entity<CatTipoArchivo>(entity =>
        {
            entity.HasKey(e => e.IdTipoArchivo);

            entity.ToTable("CatTipoArchivo", tb => tb.HasComment("Catálogo de tipos de archivos, referidos al contenido, no a su formato, por ejemplo:\r\nLey Ingresos\r\nOficios de entrega de claves catastrales\r\nProyectos de inversión\r\nCapas de información cartográfica de tipos de suelo"));

            entity.Property(e => e.IdTipoArchivo).HasColumnName("idTipoArchivo");
            entity.Property(e => e.IdExtension).HasColumnName("idExtension");
            entity.Property(e => e.IdTablaAdjunta).HasColumnName("idTablaAdjunta");
            entity.Property(e => e.TipoArchivo).HasColumnName("tipoArchivo");

            entity.HasOne(d => d.IdExtensionNavigation).WithMany(p => p.CatTipoArchivos)
                .HasForeignKey(d => d.IdExtension)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CatTipoArchivo_CatExtensiones");

            entity.HasOne(d => d.IdTablaAdjuntaNavigation).WithMany(p => p.CatTipoArchivos)
                .HasForeignKey(d => d.IdTablaAdjunta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CatTipoArchivo_TablaAdjunta");
        });

        modelBuilder.Entity<ClasificacionTema>(entity =>
        {
            entity.HasKey(e => e.IdCt);

            entity.ToTable("ClasificacionTema", tb => tb.HasComment("Contiene todos los Temas y sus clasificaciones.\r\n(Segunda Rama)\r\nDebe existir como mínimo una clasificación por cada Tema\r\nLos responsables de los temas pueden crear nuevas clasificaciones\r\n\r\nAl crear una nueva segunda rama Tema-Clasificacion se deberá:\r\n* Crear un nuevo grupo (tercera rama)  que pertenezca a la segunda Rama.\r\n* A las ramas creadas (segunda y tercera) se pondrá como responsable al creador."));

            entity.Property(e => e.IdCt).HasColumnName("idCT");
            entity.Property(e => e.IdClasificacion).HasColumnName("idClasificacion");
            entity.Property(e => e.IdTema).HasColumnName("idTema");

            entity.HasOne(d => d.IdClasificacionNavigation).WithMany(p => p.ClasificacionTemas)
                .HasForeignKey(d => d.IdClasificacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClasificacionTema_CatClasificacion");

            entity.HasOne(d => d.IdTemaNavigation).WithMany(p => p.ClasificacionTemas)
                .HasForeignKey(d => d.IdTema)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClasificacionTema_CatTema");
        });

        modelBuilder.Entity<GrupoClasificacionTema>(entity =>
        {
            entity.HasKey(e => e.IdGct);

            entity.ToTable("GrupoClasificacionTema", tb => tb.HasComment("Contiene todos los Temas y sus clasificaciones.\r\n(Segunda Rama)\r\nDebe existir como mínimo una clasificación por cada Tema\r\nLos responsables de las segundas ramas pueden crear nuevos grupos para esa rama.\r\n\r\nAl crear una nueva tercera rama Tema-Clasificacion-Grupo se pondrá como responsable al creador."));

            entity.Property(e => e.IdGct).HasColumnName("idGCT");
            entity.Property(e => e.IdCt).HasColumnName("idCT");
            entity.Property(e => e.IdGrupo).HasColumnName("idGrupo");

            entity.HasOne(d => d.IdCtNavigation).WithMany(p => p.GrupoClasificacionTemas)
                .HasForeignKey(d => d.IdCt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GrupoClasificacionTema_ClasificacionTema");

            entity.HasOne(d => d.IdGrupoNavigation).WithMany(p => p.GrupoClasificacionTemas)
                .HasForeignKey(d => d.IdGrupo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GrupoClasificacionTema_CatGrupo");
        });

        modelBuilder.Entity<MetadatoPorArchivo>(entity =>
        {
            entity.HasKey(e => e.IdmetadatoArchivo);

            entity.ToTable("MetadatoPorArchivo", tb => tb.HasComment("Contiene los valores de los metadatos de cada archivo digital subido al servidor de aechivos."));

            entity.Property(e => e.IdmetadatoArchivo).HasColumnName("idmetadatoArchivo");
            entity.Property(e => e.IdArchivo).HasColumnName("idArchivo");
            entity.Property(e => e.IdMetadato).HasColumnName("idMetadato");
            entity.Property(e => e.ValorMetadato)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("valorMetadato");

            entity.HasOne(d => d.IdArchivoNavigation).WithMany(p => p.MetadatoPorArchivos)
                .HasForeignKey(d => d.IdArchivo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MetadatoPorArchivo_Archivos");

            entity.HasOne(d => d.IdMetadatoNavigation).WithMany(p => p.MetadatoPorArchivos)
                .HasForeignKey(d => d.IdMetadato)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MetadatoPorArchivo_CatMetadato");
        });

        modelBuilder.Entity<TablaAdjuntum>(entity =>
        {
            entity.HasKey(e => e.IdTablaAdjunta).HasName("PK_camposClave");

            entity.ToTable(tb => tb.HasComment("Contiene los campos a buscar de la tabla adjunta por tipo de documento"));

            entity.Property(e => e.IdTablaAdjunta).HasColumnName("idTablaAdjunta");
            entity.Property(e => e.NombreCampo)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("nombreCampo");
            entity.Property(e => e.NombreTabla)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombreTabla");
            entity.Property(e => e.Tipo)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<TemaUsuario>(entity =>
        {
            entity.HasKey(e => e.IdTemaUsuario);

            entity.ToTable("TemaUsuario", tb => tb.HasComment("Indica quienes son los responsables de cada tema, tiene la capacidad de crear nuevas clasificaciones del Tema"));

            entity.Property(e => e.IdTemaUsuario).HasColumnName("idTemaUsuario");
            entity.Property(e => e.IdTema).HasColumnName("idTema");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

            entity.HasOne(d => d.IdTemaNavigation).WithMany(p => p.TemaUsuarios)
                .HasForeignKey(d => d.IdTema)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TemaUsuario_CatTema");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.TemaUsuarios)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TemaUsuario_Usuarios");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);

            entity.ToTable(tb => tb.HasComment("Usuarios de MIRI\r\nel numUsuario es rescatado de GUSIT"));

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.AMaterno)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("aMaterno");
            entity.Property(e => e.APaterno)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("aPaterno");
            entity.Property(e => e.Contraseña)
                .HasMaxLength(50)
                .HasColumnName("contraseña");
            entity.Property(e => e.FechaIngreso)
                .HasColumnType("datetime")
                .HasColumnName("fechaIngreso");
            entity.Property(e => e.Habilitado).HasColumnName("habilitado");
            entity.Property(e => e.IdPerfil).HasColumnName("idPerfil");
            entity.Property(e => e.Nombre)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Usuario1)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("usuario");

            entity.HasOne(d => d.IdPerfilNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdPerfil)
                .HasConstraintName("FK_Usuarios_CatPerfiles");
        });

        modelBuilder.Entity<UsuariosCt>(entity =>
        {
            entity.HasKey(e => e.IdClasifUsuario).HasName("PK_ClasificacionTemaUsuario");

            entity.ToTable("UsuariosCT", tb => tb.HasComment("Contiene  los responsables de las segundas ramas\r\nPueden crear terceras ramas\r\nPuede agregar responsables a esta segunda rama"));

            entity.Property(e => e.IdClasifUsuario).HasColumnName("idClasifUsuario");
            entity.Property(e => e.IdCt).HasColumnName("idCT");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

            entity.HasOne(d => d.IdCtNavigation).WithMany(p => p.UsuariosCts)
                .HasForeignKey(d => d.IdCt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClasificacionTemaUsuario_ClasificacionTema");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuariosCts)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsuariosCT_Usuarios");
        });

        modelBuilder.Entity<UsuariosGct>(entity =>
        {
            entity.HasKey(e => e.IdUgct);

            entity.ToTable("UsuariosGCT", tb => tb.HasComment("Contiene  los responsables del contenido de las terceras ramas\r\nEl campo permiso indica\r\n* 0 puede leer y descargar\r\n* 1 puede subir contenido + 0\r\n* 2 Puede borrar + 1"));

            entity.Property(e => e.IdUgct).HasColumnName("idUGCT");
            entity.Property(e => e.IdGct).HasColumnName("idGCT");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Permiso).HasColumnName("permiso");

            entity.HasOne(d => d.IdGctNavigation).WithMany(p => p.UsuariosGcts)
                .HasForeignKey(d => d.IdGct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsuariosGCT_GrupoClasificacionTema");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuariosGcts)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsuariosGCT_Usuarios");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
