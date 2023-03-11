using APIMiri.Data;
using APIMiri.Models;
using APIMiri.Models.db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace APIMiri.Controllers
{
    [ApiController]
    [Route("grupoController")]
    public class GrupoController : Controller
    {
        private readonly DbMiriContext _dbContext;
        private respuestaAPIMiri msj = new respuestaAPIMiri();

        public GrupoController(DbMiriContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("esGrupo/{nameGrupo}")]
        public async Task<ActionResult<bool>> GetEsClasif(string nameGrupo)
        {
            var existeGrupo = await _dbContext.CatGrupos.Where(c => c.Grupo == nameGrupo).FirstOrDefaultAsync<CatGrupo>();
            if (existeGrupo is null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        [HttpGet("readUsuariosSinGrupo/{idgrupo}")]
        public async Task<ActionResult<List<MUsuariosSinDirectorio>>> Get(int idgrupo)
        {
            List<MUsuariosSinDirectorio> must = new List<MUsuariosSinDirectorio>();
            var obtenerIDGCT = _dbContext.GrupoClasificacionTemas.Where(c => c.IdGrupo == idgrupo).Select(c => c.IdGct).FirstOrDefault();
            if (obtenerIDGCT > 0)
            {
                var query = await _dbContext.Usuarios.Where(t => !t.UsuariosGcts.Any(d => d.IdUsuario == t.IdUsuario && d.IdGct == obtenerIDGCT)).ToListAsync();
                foreach (var item in query)
                {
                    must.Add(new MUsuariosSinDirectorio { idUsuario = item.IdUsuario, usuario = item.Usuario1 });
                }

            }

            return must;
        }
        [HttpGet("readGrupo/{idtema}/{idclasif}/{iduser}")]
        public async Task<ActionResult<IEnumerable<CatGrupo>>> Get(int idtema,int idclasif,int iduser)
        {
            var query = await (from cc in _dbContext.CatGrupos
                               join gct in _dbContext.GrupoClasificacionTemas on cc.IdGrupo equals gct.IdGrupo
                               join ct in _dbContext.ClasificacionTemas on gct.IdCt equals ct.IdCt
                               join ugct in _dbContext.UsuariosGcts on gct.IdGct equals ugct.IdGct
                               where ct.IdTema == idtema && ct.IdClasificacion == idclasif && ugct.IdUsuario == iduser
                               select new CatGrupo
                               {
                                  IdGrupo = cc.IdGrupo,
                                  Grupo = cc.Grupo,
                                  IdTipoArchivo = cc.IdTipoArchivo
                               }).ToListAsync();
            return query;
        }
        [HttpPost("createGrupoClasificacionTema")]
        public async Task<ActionResult<respuestaAPIMiri>> Post(MGrupoCT _grupoClasificacionTema)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (_grupoClasificacionTema._idClasificacion <= 0 || _grupoClasificacionTema._idUser <= 0)
                    {
                        msj.codigo = -300;
                        msj.Descripcion = "ID CLASIFICACIÓN Ó ID USUARIO NO VALIDO";
                    }
                    else
                    {
                        var existeUser = await _dbContext.Usuarios.FindAsync(_grupoClasificacionTema._idUser);
                        if (existeUser != null)
                        {
                            var existeGrupo = _dbContext.CatGrupos.Where(c => c.Grupo == _grupoClasificacionTema._nombreGrupo).FirstOrDefault<CatGrupo>();
                            if (existeGrupo is null)
                            {
                                var g = new CatGrupo
                                {
                                    Grupo = _grupoClasificacionTema._nombreGrupo
                                };
                                _dbContext.CatGrupos.Add(g);
                                await _dbContext.SaveChangesAsync();
                                var obtenerIDCT = _dbContext.ClasificacionTemas.Where(c => c.IdClasificacion == _grupoClasificacionTema._idClasificacion).Select(c => c.IdCt).FirstOrDefault();
                                var obtenerIDGrupo = _dbContext.CatGrupos.Where(c => c.Grupo == _grupoClasificacionTema._nombreGrupo).Select(c => c.IdGrupo).FirstOrDefault();
                               
                                    var gCT = new GrupoClasificacionTema
                                    {
                                        IdCt = obtenerIDCT,
                                        IdGrupo = obtenerIDGrupo
                                    };

                                    _dbContext.GrupoClasificacionTemas.Add(gCT);
                                    await _dbContext.SaveChangesAsync();
                                    var obtenerIDGCT = _dbContext.GrupoClasificacionTemas.Where(c => c.IdGrupo == obtenerIDGrupo).Select(c => c.IdGct).FirstOrDefault();

                                    var ugct = new UsuariosGct
                                    {
                                        IdUsuario = _grupoClasificacionTema._idUser,
                                        Permiso = _grupoClasificacionTema._permiso,
                                        IdGct = obtenerIDGCT
                                    };
                                    _dbContext.UsuariosGcts.Add(ugct);
                                    await _dbContext.SaveChangesAsync();
                                    dbContextTransaction.Commit();
                                    msj.codigo = 111;
                                    msj.Descripcion = "GRUPO CREADO CON EXITO";
                              
                            }
                            else
                            {
                                msj.codigo = 222;
                                msj.Descripcion = "EL GRUPO YA EXISTE EN EL CATALOGO DE GRUPOS";
                            }
                        }
                        else
                        {
                            msj.codigo = 333;
                            msj.Descripcion = "ID USUARIO NO EXISTE";
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    msj.codigo = -200;
                    msj.Descripcion = "PROBLEMAS CON EL SERVIDOR - Error: " + ex;
                }
            }
            return msj;
        }
        [HttpPut("updateGrupoPermisos/{idGrupo}/{idUser}/{permiso}")]
        public async Task<ActionResult<respuestaAPIMiri>> PutPermisos(int idGrupo,int idUser, int permiso)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (idGrupo <= 0 || idUser <= 0 || permiso <= 0)
                    {
                        msj.codigo = -300;
                        msj.Descripcion = "ID GRUPO Ó ID USUARIO O PERMISO NO VALIDO";
                    }
                    else
                    {
                        var obtenerIDGCT = _dbContext.GrupoClasificacionTemas.Where(c => c.IdGrupo == idGrupo).Select(c => c.IdGct).FirstOrDefault();
                        var updatePermisosGrupo = _dbContext.UsuariosGcts.Where(c => c.IdGct == obtenerIDGCT && c.IdUsuario == idUser).FirstOrDefault<UsuariosGct>();
                        if (obtenerIDGCT > 0)
                        {
                            if (updatePermisosGrupo != null)
                            {
                                updatePermisosGrupo.IdUsuario = idUser;
                                updatePermisosGrupo.IdGct = obtenerIDGCT;
                                updatePermisosGrupo.Permiso = permiso;
                                await _dbContext.SaveChangesAsync();
                                dbContextTransaction.Commit();
                                msj.codigo = 444;
                                msj.Descripcion = "PERMISO ACTUALIZADO CON EXITO";
                            }
                            else
                            {
                                msj.codigo = 333;
                                msj.Descripcion = "EL ID GRUPO CLASIFICACION TEMA NO EXISTE EN USUARIOSGCT";
                            }
                        }
                        else
                        {
                            msj.codigo = 333;
                            msj.Descripcion = "EL ID GRUPO NO EXISTE EN GRUPO-CLASIFICACION-TEMA";
                        }
                    }
                 
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    msj.codigo = -200;
                    msj.Descripcion = "PROBLEMAS CON EL SERVIDOR - Error: " + ex;
                }
            }
            return msj;
        }
        [HttpPut("updateGrupo")]
        public async Task<ActionResult<respuestaAPIMiri>> Put(CatGrupo _grupo)
        {

            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var updateGrupo = _dbContext.CatGrupos.Where(c => c.IdGrupo == _grupo.IdGrupo).FirstOrDefault<CatGrupo>();
                    if (updateGrupo != null)
                    {
                        updateGrupo.Grupo = _grupo.Grupo;
                        await _dbContext.SaveChangesAsync();
                        dbContextTransaction.Commit();
                        msj.codigo = 444;
                        msj.Descripcion = "NOMBRE ACTUALIZADO CON EXITO";
                    }
                    else
                    {
                        msj.codigo = 333;
                        msj.Descripcion = "EL GRUPO NO EXISTE";
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    msj.codigo = -200;
                    msj.Descripcion = "PROBLEMAS CON EL SERVIDOR - Error: " + ex;
                }
            }
            return msj;
        }
        [HttpDelete("deleteGrupo/{idgrupo}")]
        public async Task<ActionResult<respuestaAPIMiri>> DeleteGrupo( int idgrupo)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (idgrupo <= 0)
                    {
                        msj.codigo = -300;
                        msj.Descripcion = "ID GRUPO NO VALIDO";
                    }
                    else {
                        var deleteGrupoClasificacionTema = await _dbContext.GrupoClasificacionTemas.Where(c => c.IdGrupo == idgrupo).FirstOrDefaultAsync<GrupoClasificacionTema>();
                        if (deleteGrupoClasificacionTema != null)
                        {
                            var obtenerIDGCT = _dbContext.GrupoClasificacionTemas.Where(c => c.IdGrupo == idgrupo).Select(c => c.IdGct).FirstOrDefault();
                            var deleteUsuariosGCT = await _dbContext.UsuariosGcts.Where(c => c.IdGct == obtenerIDGCT).ToListAsync<UsuariosGct>();
                            if(deleteUsuariosGCT != null)
                            {

                                    _dbContext.UsuariosGcts.RemoveRange(deleteUsuariosGCT);
                                    await _dbContext.SaveChangesAsync();
                              
                               

                                _dbContext.Entry(deleteGrupoClasificacionTema).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                                await _dbContext.SaveChangesAsync();

                                var deleteGrupo = await _dbContext.CatGrupos.FindAsync(idgrupo);
                                if (deleteGrupo != null)
                                {
                                    _dbContext.Entry(deleteGrupo).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                                    await _dbContext.SaveChangesAsync();
                                    dbContextTransaction.Commit();
                                    msj.codigo = 555;
                                    msj.Descripcion = "GRUPO ELIMINADO CON EXITO";
                                }
                                else
                                {
                                    msj.codigo = 333;
                                    msj.Descripcion = "EL GRUPO NO EXISTE";
                                }
                            }
                            else
                            {
                                msj.codigo = 333;
                                msj.Descripcion = "NO EXISTE ID GRUPO-CLASIFICACION-TEMA EN USUARIOSGCT";
                            }
                        }
                        else
                        {
                            msj.codigo = 333;
                            msj.Descripcion = "NO EXISTE ID GRUPO EN GRUPO-CLASIFICACION-TEMA";
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    msj.codigo = -200;
                    msj.Descripcion = "PROBLEMAS CON EL SERVIDOR - Error: " + ex;
                }
            }
            return msj;
        }
        [HttpPost("compartirGrupo/{idgrupo}/{iduser}/{permiso}")]
        public async Task<ActionResult<respuestaAPIMiri>> Post(int idgrupo, int iduser,int permiso)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {

                    if (iduser <= 0 || permiso < 0 || permiso > 2 )
                    {
                        msj.codigo = -300;
                        msj.Descripcion = "ID USUARIO NO VALIDO";
                    }
                    else
                    {
                        var existeUser = await _dbContext.Usuarios.FindAsync(iduser);
                        if (existeUser != null)
                        {
                            var existeGrupo = await _dbContext.CatGrupos.FindAsync(idgrupo);
                            if (existeGrupo != null)
                            {
                                var obtenerIDGCT = _dbContext.GrupoClasificacionTemas.Where(c => c.IdGrupo == idgrupo).Select(c => c.IdGct).FirstOrDefault();
                                if(obtenerIDGCT > 0)
                                {
                                    var compartido = _dbContext.UsuariosGcts.Where(c => c.IdGct == obtenerIDGCT && c.IdUsuario == iduser).FirstOrDefault<UsuariosGct>();
                                    if (compartido is null)
                                    {
                                        var ugct = new UsuariosGct
                                        {
                                            IdUsuario = iduser,
                                            IdGct = obtenerIDGCT,
                                            Permiso = permiso
                                        };
                                        _dbContext.UsuariosGcts.Add(ugct);
                                        await _dbContext.SaveChangesAsync();
                                        dbContextTransaction.Commit();
                                        msj.codigo = 111;
                                        msj.Descripcion = "GRUPO COMPARTIDO CON EXITO";
                                    }
                                    else
                                    {
                                        msj.codigo = 222;
                                        msj.Descripcion = "YA SE COMPARTIÓ EL GRUPO CON ESTE USUARIO";
                                    }
                                }
                                else
                                {
                                    msj.codigo = 333;
                                    msj.Descripcion = "ID GRUPO NO EXISTE EN GRUPO-CLASIFICACION-TEMA";
                                }
                            }
                            else
                            {

                                msj.codigo = 333;
                                msj.Descripcion = "ID GRUPO NO EXISTE";
                            }
                        }
                        else
                        {
                            msj.codigo = 333;
                            msj.Descripcion = "ID USUARIO NO EXISTE";
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    msj.codigo = -200;
                    msj.Descripcion = "PROBLEMAS CON EL SERVIDOR - Error: " + ex;
                }
            }
            return msj;
        }
    }
}
