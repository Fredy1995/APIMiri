using Microsoft.AspNetCore.Mvc;
using APIMiri.Models;
using Microsoft.EntityFrameworkCore;
using APIMiri.Models.db;
using APIMiri.Data;

namespace APIMiri.Controllers
{
    [ApiController]
    [Route("clasificacionController")]
    public class ClasificacionController : Controller
    {
        private readonly DbMiriContext _dbContext;
        private respuestaAPIMiri msj = new respuestaAPIMiri();
       
        public ClasificacionController(DbMiriContext dbContext)
        {
            _dbContext = dbContext;

        }
        
        [HttpGet("devuelveObjTema/{idClasif}")]
        public async Task<ActionResult<List<MTemas>>> GetTema(int idClasif)
        {
            var query = await (from c in _dbContext.CatClasificacions
                               join ct in _dbContext.ClasificacionTemas on c.IdClasificacion equals ct.IdClasificacion
                               join t in _dbContext.CatTemas on ct.IdTema equals t.IdTema
                               where c.IdClasificacion == idClasif
                               select new MTemas
                               {
                                  IdTema =  t.IdTema,
                                  Tema = t.Tema
                               }).ToListAsync();
            return query;
        }
        [HttpGet("esClasificacion/{nameClasif}")]
        public async Task<ActionResult<bool>> GetEsClasif(string nameClasif)
        {
            var existeClasif = await _dbContext.CatClasificacions.Where(c => c.Clasificacion == nameClasif).FirstOrDefaultAsync<CatClasificacion>();
            if (existeClasif is null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
       
        [HttpGet("readUsuariosSinClasif/{idclasif}")]
        public async Task<ActionResult<List<MUsuariosSinDirectorio>>> Get(int idclasif)
        {
            List<MUsuariosSinDirectorio> musc = new List<MUsuariosSinDirectorio>();
            var obtenerIDCT = _dbContext.ClasificacionTemas.Where(c => c.IdClasificacion == idclasif).Select(c => c.IdCt).FirstOrDefault();
            if(obtenerIDCT > 0)
            {
                var query = await _dbContext.Usuarios.Where(t => !t.UsuariosCts.Any(d => d.IdUsuario == t.IdUsuario && d.IdCt == obtenerIDCT)).ToListAsync();
                foreach (var item in query)
                {
                    musc.Add(new MUsuariosSinDirectorio { idUsuario = item.IdUsuario, usuario = item.Usuario1 });
                }

            }

            return musc;
        }
        [HttpGet("readClasificaciones/{idtema}/{iduser}")]
        public async Task<ActionResult<List<MClasificaciones>>> Get(int idtema, int iduser)
        {
          
              var  query = await (from cc in _dbContext.CatClasificacions
                                   join ct in _dbContext.ClasificacionTemas on cc.IdClasificacion equals ct.IdClasificacion
                                   join uct in _dbContext.UsuariosCts on ct.IdCt equals uct.IdCt
                                   where ct.IdTema == idtema && uct.IdUsuario == iduser
                                   select new MClasificaciones
                                   {
                                       idClasif = cc.IdClasificacion,
                                       Clasificacion = cc.Clasificacion
                                   }).ToListAsync();
            
                
            return query;
        }
        [HttpPost("createClasificacionTema")]
        public async Task<ActionResult<respuestaAPIMiri>> Post(MClasificiacionTema _clasificacionTema)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (_clasificacionTema._idTema <= 0 || _clasificacionTema._idUser <= 0)
                    {
                        msj.codigo = -300;
                        msj.Descripcion = "ID TEMA Ó ID USUARIO NO VALIDO";
                    }
                    else
                    {
                        var existeUser = await _dbContext.Usuarios.FindAsync(_clasificacionTema._idUser);
                        if (existeUser != null)
                        {
                            var existeClasificacion = _dbContext.CatClasificacions.Where(c => c.Clasificacion == _clasificacionTema._nombreClasificacion).FirstOrDefault<CatClasificacion>();
                            if (existeClasificacion is null)
                            {
                                var c = new CatClasificacion
                                {
                                    Clasificacion = _clasificacionTema._nombreClasificacion,
                                    Propietario = await _dbContext.Usuarios.Where(c => c.IdUsuario == _clasificacionTema._idUser).Select(c => c.Nombre + " " + c.APaterno + " " + c.AMaterno).FirstOrDefaultAsync(),
                                    FechaCreacion = DateTime.Now
                            };
                                _dbContext.CatClasificacions.Add(c);
                                await _dbContext.SaveChangesAsync();
                                var obtenerIDClasificacion = _dbContext.CatClasificacions.Where(c => c.Clasificacion == _clasificacionTema._nombreClasificacion).Select(c => c.IdClasificacion).FirstOrDefault();
                                var existeIdTema = await _dbContext.CatTemas.FindAsync(_clasificacionTema._idTema);
                                if(existeIdTema != null)
                                {
                                    var cT = new ClasificacionTema
                                    {
                                        IdTema = _clasificacionTema._idTema,
                                        IdClasificacion = obtenerIDClasificacion
                                    };

                                    _dbContext.ClasificacionTemas.Add(cT);
                                    await _dbContext.SaveChangesAsync();

                                    var obtenerIDCT = _dbContext.ClasificacionTemas.Where(c => c.IdClasificacion == obtenerIDClasificacion).Select(c => c.IdCt).FirstOrDefault();
                                    
                                    var uct = new UsuariosCt
                                     {
                                            IdUsuario = _clasificacionTema._idUser,
                                            IdCt = obtenerIDCT
                                     };
                                    _dbContext.UsuariosCts.Add(uct);
                                    /////////////////////////////////////////////////COMPARTIR A ADMIN
                                    var perfil = await _dbContext.Usuarios.Where(c => c.IdUsuario == _clasificacionTema._idUser).Select(c => c.IdPerfil).FirstOrDefaultAsync();
                                    if (perfil != null && perfil == 2) // si el perfil es visualizador de contenido, la clasificación es compartida al Admin
                                    {
                                        var uct2 = new UsuariosCt
                                        {
                                            IdUsuario = 46, // La clasificación creada por cualquier usuario es compartida al admin para mejor control
                                            IdCt = obtenerIDCT
                                        };
                                        _dbContext.UsuariosCts.Add(uct2);
                                    }
                                    /////////////////////////////////////////////////COMPARTIR A ADMIN
                                    await _dbContext.SaveChangesAsync();
                                    dbContextTransaction.Commit();
                                    msj.codigo = 111;
                                    msj.Descripcion = "CLASIFICACION CREADA CON EXITO";
                                }
                                else
                                {
                                    msj.codigo = 333;
                                    msj.Descripcion = "ID TEMA NO EXISTE";
                                }
                            }
                            else
                            {
                                msj.codigo = 222;
                                msj.Descripcion = "EL NOMBRE DE LA CLASIFICACIÓN YA EXISTE EN EL CATALOGO DE CLASIFICACIONES";
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

        [HttpPut("updateClasificacion")]
        public async Task<ActionResult<respuestaAPIMiri>> Put(MClasificaciones _clasificacion)
        {

            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var updateClasificacion = _dbContext.CatClasificacions.Where(c => c.IdClasificacion == _clasificacion.idClasif).FirstOrDefault<CatClasificacion>();
                    if (updateClasificacion != null)
                    {
                        var existeNombreClasif = _dbContext.CatClasificacions.Where(c => c.Clasificacion == _clasificacion.Clasificacion).FirstOrDefault<CatClasificacion>(); 
                        if(existeNombreClasif is null)
                        {
                            updateClasificacion.Clasificacion = _clasificacion.Clasificacion;
                            updateClasificacion.ModificadoPor = await _dbContext.Usuarios.Where(c => c.IdUsuario == _clasificacion.IdUser).Select(c => c.Nombre + " " + c.APaterno + " " + c.AMaterno).FirstOrDefaultAsync();
                            updateClasificacion.FechaModificacion = DateTime.Now;
                            await _dbContext.SaveChangesAsync();
                            dbContextTransaction.Commit();
                            msj.codigo = 444;
                            msj.Descripcion = "NOMBRE ACTUALIZADO CON EXITO";
                        }
                        else
                        {
                            msj.codigo = 222;
                            msj.Descripcion = "EL NOMBRE DE LA CLASIFICACIÓN YA EXISTE EN EL CATALOGO DE CLASIFICACIONES";
                        }
                        
                    }
                    else
                    {
                        msj.codigo = 333;
                        msj.Descripcion = "LA CLASIFICACIÓN NO EXISTE";
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

        [HttpDelete("deleteClasificacion/{idclasificacion}")]
        public async Task<ActionResult<respuestaAPIMiri>> DeleteClasificacion( int idclasificacion)
        {
            GrupoController _grupoController = new GrupoController(_dbContext);
            //Obtener todos los grupos que pertenecen a la clasificación seleccionada para eliminar
            //Este codigo lo pongo afuera de la transacción porque el metodo DeleteGrupo tiene otra transacción y marca error al contener una transaccion en otra.
            var ListidG = await (from g in _dbContext.CatGrupos
                                 join gct in _dbContext.GrupoClasificacionTemas on g.IdGrupo equals gct.IdGrupo
                                 join ct in _dbContext.ClasificacionTemas on gct.IdCt equals ct.IdCt
                                 where ct.IdClasificacion == idclasificacion
                                 select new GrupoClasificacionTema
                                 {
                                     IdGrupo = gct.IdGrupo
                                 }).ToListAsync();
            if (ListidG.Count > 0)
            {
                //Aqui elimino todos los grupos que pertenecen en la clasificación
                foreach (var item in ListidG)
                {
                    await _grupoController.DeleteGrupo(item.IdGrupo);
                }
            }
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {

                    if (idclasificacion <= 0)
                    {
                        msj.codigo = -300;
                        msj.Descripcion = "ID CLASIFICACIÓN NO VALIDO";
                    }
                    else
                    {
                       

                        var deleteClasificacionTema = await _dbContext.ClasificacionTemas.Where(c => c.IdClasificacion == idclasificacion).FirstOrDefaultAsync<ClasificacionTema>();
                        if(deleteClasificacionTema != null)
                        {
                            var obtenerIDCT = _dbContext.ClasificacionTemas.Where(c => c.IdClasificacion == idclasificacion).Select(c => c.IdCt).FirstOrDefault();
                            var deleteUsuariosCT = await _dbContext.UsuariosCts.Where(c => c.IdCt == obtenerIDCT).ToListAsync<UsuariosCt>();
                            if(deleteUsuariosCT != null)
                            {
                                _dbContext.UsuariosCts.RemoveRange(deleteUsuariosCT);
                                await _dbContext.SaveChangesAsync();
                               
                               
                                    _dbContext.Entry(deleteClasificacionTema).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                                    await _dbContext.SaveChangesAsync();

                                    var deleteClasificacion = await _dbContext.CatClasificacions.FindAsync(idclasificacion);
                                    if (deleteClasificacion != null)
                                    {
                                        _dbContext.Entry(deleteClasificacion).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                                        await _dbContext.SaveChangesAsync();
                                        dbContextTransaction.Commit();
                                        msj.codigo = 555;
                                        msj.Descripcion = "CLASIFICIACIÓN ELIMINADA CON EXITO";
                                    }
                                    else
                                    {
                                        msj.codigo = 333;
                                        msj.Descripcion = "LA CLASIFICACIÓN NO EXISTE";
                                    }
                                
                            }
                            else
                            {
                                msj.codigo = 333;
                                msj.Descripcion = "NO EXISTE ID CLASIFICACION TEMA EN USUARIOSCT";
                            }
                        }
                        else
                        {
                            msj.codigo = 333;
                            msj.Descripcion = "NO EXISTE ID CLASIFICACION EN CLASIFICACION-TEMA";
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

        [HttpPost("compartirClasificacion")]
        public async Task<ActionResult<respuestaAPIMiri>> Post(MCompartir _compartir)
        {
            GrupoController _gcontroller = new GrupoController(_dbContext);
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var iduser = await _dbContext.Usuarios.Where(c => c.Usuario1 == _compartir._username).Select(c => c.IdUsuario).FirstOrDefaultAsync();
                    if (iduser <= 0)
                    {
                        msj.codigo = -300;
                        msj.Descripcion = "ID USUARIO NO VALIDO";
                    }
                    else
                    {
                        var existeUser = await _dbContext.Usuarios.FindAsync(iduser);
                        if (existeUser != null)
                        {
                            var existeClasif = await _dbContext.CatClasificacions.FindAsync(_compartir._idDirectorio);
                            if (existeClasif != null)
                            {
                                var obtenerIDCT = _dbContext.ClasificacionTemas.Where(c => c.IdClasificacion == _compartir._idDirectorio).Select(c => c.IdCt).FirstOrDefault();
                                if(obtenerIDCT > 0)
                                {
                                    var compartido = _dbContext.UsuariosCts.Where(c => c.IdCt == obtenerIDCT && c.IdUsuario == iduser).FirstOrDefault<UsuariosCt>();
                                    if (compartido is null)
                                    {
                                        var uct = new UsuariosCt
                                        {
                                            IdUsuario = iduser,
                                            IdCt = obtenerIDCT
                                        };
                                        _dbContext.UsuariosCts.Add(uct);
                                        //Obtener todos los grupos que pertenecen a cada clasificación
                                        var ListidGCT = await (from g in _dbContext.CatGrupos
                                                               join gct in _dbContext.GrupoClasificacionTemas on g.IdGrupo equals gct.IdGrupo
                                                               join ct in _dbContext.ClasificacionTemas on gct.IdCt equals ct.IdCt
                                                               where ct.IdClasificacion == _compartir._idDirectorio
                                                               select new GrupoClasificacionTema
                                                               {
                                                                   IdGct = gct.IdGct
                                                               }).ToListAsync();
                                        foreach (var item2 in ListidGCT)
                                        {
                                            ////Verificar que el usuario y grupo existen en la tabla UsuariosGCT
                                            var existeUsuarioGCT = await _dbContext.UsuariosGcts.Where(c => c.IdUsuario == iduser && c.IdGct == item2.IdGct).FirstOrDefaultAsync();
                                            if (existeUsuarioGCT == null)
                                            {
                                                var ugct = new UsuariosGct
                                                {
                                                    IdUsuario = iduser,
                                                    Permiso = 0, //Con permiso 0 - solo puede Leer y descargar
                                                    IdGct = item2.IdGct
                                                };
                                                _dbContext.UsuariosGcts.Add(ugct);
                                            }

                                        }
                                        await _dbContext.SaveChangesAsync();
                                        dbContextTransaction.Commit();
                                        msj.codigo = 111;
                                        msj.Descripcion = "CLASIFICACIÓN COMPARTIDA CON EXITO";
                                    }
                                    else
                                    {
                                        msj.codigo = 222;
                                        msj.Descripcion = "YA SE COMPARTIÓ LA CLASIFICACIÓN CON ESTE USUARIO";
                                    }
                                }
                                else
                                {
                                    msj.codigo = 333;
                                    msj.Descripcion = "ID CLASIFICIACIÓN NO EXISTE EN CLASIFICIACION-TEMA";
                                }
                            }
                            else
                            {

                                msj.codigo = 333;
                                msj.Descripcion = "ID CLASIFICIACIÓN NO EXISTE";
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
        [HttpDelete("quitarAccesoClasificacion/{idDirectorio}/{userName}")]
        public async Task<ActionResult<respuestaAPIMiri>> DeleteAquitarAccesoClasificacion(int idDirectorio, string userName)
        {
            GrupoController _grupoController = new GrupoController(_dbContext);
            //Obtener todos los grupos que pertenecen a la clasificación seleccionada para quitar acceso
            //Este codigo lo pongo afuera de la transacción porque el metodo DeleteGrupo tiene otra transacción y marca error al contener una transaccion en otra.
            var ListidG = await (from g in _dbContext.CatGrupos
                                 join gct in _dbContext.GrupoClasificacionTemas on g.IdGrupo equals gct.IdGrupo
                                 join ct in _dbContext.ClasificacionTemas on gct.IdCt equals ct.IdCt
                                 where ct.IdClasificacion == idDirectorio
                                 select new GrupoClasificacionTema
                                 {
                                     IdGrupo = gct.IdGrupo
                                 }).ToListAsync();
            if (ListidG.Count > 0)
            {
                //Aqui aqui el acceso a todos los grupos que pertenecen en la clasificación
                foreach (var item in ListidG)
                {
                    await _grupoController.DeleteQuitarAccesoGrupo(item.IdGrupo, userName);
                }
            }

            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var iduser = await _dbContext.Usuarios.Where(c => c.Usuario1 == userName).Select(c => c.IdUsuario).FirstOrDefaultAsync();
                    if (iduser <= 0)
                    {
                        msj.codigo = -300;
                        msj.Descripcion = "ID USUARIO NO VALIDO";
                    }
                    else
                    {
                       

                        var existeUser = await _dbContext.Usuarios.FindAsync(iduser);
                        if (existeUser != null)
                        {
                            var existeClasif = await _dbContext.CatClasificacions.FindAsync(idDirectorio);
                            if (existeClasif != null)
                            {

                                var obtenerIDCT = _dbContext.ClasificacionTemas.Where(c => c.IdClasificacion == idDirectorio).Select(c => c.IdCt).FirstOrDefault();
                                if (obtenerIDCT > 0)
                                {
                                    var deleteUsuariosCT = await _dbContext.UsuariosCts.Where(c => c.IdCt == obtenerIDCT && c.IdUsuario == iduser).ToListAsync<UsuariosCt>();
                                    if (deleteUsuariosCT != null)
                                    {
                                        _dbContext.UsuariosCts.RemoveRange(deleteUsuariosCT);
                                        await _dbContext.SaveChangesAsync();
                                        dbContextTransaction.Commit();
                                        msj.codigo = 666;
                                        msj.Descripcion = "SE DEJO DE COMPARTIR EL DIRECTORIO";
                                    }
                                    else
                                    {
                                        msj.codigo = 333;
                                        msj.Descripcion = "NO EXISTE ID CLASIFICACION TEMA EN USUARIOSCT";
                                    }
                                }
                                else
                                {
                                    msj.codigo = 333;
                                    msj.Descripcion = "ID CLASIFICIACIÓN NO EXISTE EN CLASIFICIACION-TEMA";
                                }
                            }
                            else
                            {

                                msj.codigo = 333;
                                msj.Descripcion = "ID CLASIFICIACIÓN NO EXISTE";
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
