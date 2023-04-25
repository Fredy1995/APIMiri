using APIMiri.Data;
using APIMiri.Models;
using APIMiri.Models.db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIMiri.Controllers
{
    [ApiController]
    [Route("temaController")]
    public class TemaController : Controller
    {
        private readonly DbMiriContext _dbContext;
        private respuestaAPIMiri msj = new respuestaAPIMiri();
        public TemaController(DbMiriContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("esTema/{nameTema}")]
        public async Task<ActionResult<bool>> GetEsTema(string nameTema)
        {
            var existeTema = await _dbContext.CatTemas.Where(c => c.Tema == nameTema).FirstOrDefaultAsync<CatTema>();
            if (existeTema is null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
       
        [HttpGet("readUsuariosSinTema/{idtema}")]
        public async Task<ActionResult<List<MUsuariosSinDirectorio>>> Get(int idtema)
        {
            List<MUsuariosSinDirectorio> must = new List<MUsuariosSinDirectorio>();
            var existeTema = await _dbContext.CatTemas.FindAsync(idtema);
            if (existeTema != null)
            {
                var query = await _dbContext.Usuarios.Where(t => !t.TemaUsuarios.Any(d => d.IdUsuario == t.IdUsuario && d.IdTema == idtema)).ToListAsync();
                foreach (var item in query)
                {
                    must.Add(new MUsuariosSinDirectorio { idUsuario = item.IdUsuario, usuario = item.Usuario1 });
                }
            }
         

            return must;
        }
        [HttpGet("readTemas/{iduser}")]
        public async Task<ActionResult<List<MTemas>>> GetTemas(int iduser)
        {
                var query = await (from ct in _dbContext.CatTemas
                                   join tu in _dbContext.TemaUsuarios on ct.IdTema equals tu.IdTema
                                   where tu.IdUsuario == iduser
                                   select new MTemas
                                   {
                                       IdTema = ct.IdTema,
                                       Tema = ct.Tema
                                   }).ToListAsync();
          
            return query;
        }
     
        [HttpPost("createTemaUsuario")]
        public async Task<ActionResult<respuestaAPIMiri>> Post(MTemaUsuario _temaUsuario)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (_temaUsuario._iduser <= 0)
                    {
                        msj.codigo = -300;
                        msj.Descripcion = "ID USUARIO NO VALIDO";
                    }
                    else
                    {
                        var existeUser = await _dbContext.Usuarios.FindAsync(_temaUsuario._iduser);
                        if(existeUser != null)
                        {
                            var existeTema = _dbContext.CatTemas.Where(c => c.Tema == _temaUsuario._tema).FirstOrDefault<CatTema>();
                            if (existeTema is null)
                            {
                                var t = new CatTema
                                {
                                    Tema = _temaUsuario._tema,
                                    Propietario = await _dbContext.Usuarios.Where(c => c.IdUsuario == _temaUsuario._iduser).Select(c => c.Nombre + " " + c.APaterno +" "+c.AMaterno).FirstOrDefaultAsync(),
                                    FechaCreacion = DateTime.Now
                                };
                                _dbContext.CatTemas.Add(t);
                                await _dbContext.SaveChangesAsync();
                                var obtenerIDTema = _dbContext.CatTemas.Where(c => c.Tema == _temaUsuario._tema).Select(c => c.IdTema).FirstOrDefault();
                                var tU = new TemaUsuario
                                {
                                    IdUsuario = _temaUsuario._iduser,
                                    IdTema = obtenerIDTema
                                };

                                _dbContext.TemaUsuarios.Add(tU);
                                await _dbContext.SaveChangesAsync();
                                dbContextTransaction.Commit();
                                msj.codigo = 111;
                                msj.Descripcion = "TEMA CREADO CON EXITO";
                            }
                            else
                            {
                                msj.codigo = 222;
                                msj.Descripcion = "EL TEMA YA EXISTE";
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
        [HttpPut("updateTema")]
        public async Task<ActionResult<respuestaAPIMiri>> Put(MTemas _tema)
        {

            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var updateTema = _dbContext.CatTemas.Where(c => c.IdTema == _tema.IdTema).FirstOrDefault<CatTema>();
                    if (updateTema != null)
                    {
                        var existeNombreTema = _dbContext.CatTemas.Where(c => c.Tema == _tema.Tema).FirstOrDefault<CatTema>();
                        if(existeNombreTema is null)
                        {
                            updateTema.Tema = _tema.Tema;
                            updateTema.ModificadoPor = await _dbContext.Usuarios.Where(c => c.IdUsuario == _tema.IdUser).Select(c => c.Nombre + " " + c.APaterno + " " + c.AMaterno).FirstOrDefaultAsync();
                            updateTema.FechaModificacion = DateTime.Now;
                            await _dbContext.SaveChangesAsync();
                            dbContextTransaction.Commit();
                            msj.codigo = 444;
                            msj.Descripcion = "NOMBRE ACTUALIZADO CON EXITO";
                        }
                        else
                        {
                            msj.codigo = 222;
                            msj.Descripcion = "EL NOMBRE DE TEMA YA EXISTE EN EL CATALOGO DE TEMAS";
                        }
                      
                    }
                    else
                    {
                        msj.codigo = 333;
                        msj.Descripcion = "EL TEMA NO EXISTE";
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

        [HttpDelete("deleteTema/{idtema}")]
        public async Task<ActionResult<respuestaAPIMiri>> Delete(int idtema)
        {
            ClasificacionController _clasifcontroller = new ClasificacionController(_dbContext);
            //Obtener todas las clasificaciones que pertencen a tema
            var ListidClasif = await (from cc in _dbContext.CatClasificacions
                                  join ct in _dbContext.ClasificacionTemas on cc.IdClasificacion equals ct.IdClasificacion
                                  join t in _dbContext.CatTemas on ct.IdTema equals t.IdTema
                                  where t.IdTema == idtema
                                  select new ClasificacionTema
                                  {
                                      IdClasificacion = ct.IdClasificacion
                                  }).ToListAsync();
            if(ListidClasif.Count > 0)
            {
                //Aqui elimino todos las clasificaciones que pertenecen a TEMA
                foreach (var item in ListidClasif)
                {
                    await _clasifcontroller.DeleteClasificacion(item.IdClasificacion);
                }
            }

            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {

                    if (idtema <= 0)
                    {
                        msj.codigo = -300;
                        msj.Descripcion = "ID TEMA NO VALIDO";
                    }
                    else
                    {
                       
                        var deleteTemaUsuario = await _dbContext.TemaUsuarios.Where(c => c.IdTema == idtema).ToListAsync<TemaUsuario>();

                        if (deleteTemaUsuario != null)
                        {
                            _dbContext.TemaUsuarios.RemoveRange(deleteTemaUsuario);
                            await _dbContext.SaveChangesAsync();

                            var deleteTema = await _dbContext.CatTemas.FindAsync(idtema);
                            if (deleteTema != null)
                            {
                                _dbContext.Entry(deleteTema).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                                await _dbContext.SaveChangesAsync();
                                dbContextTransaction.Commit();
                                msj.codigo = 555;
                                msj.Descripcion = "TEMA ELIMINADO CON EXITO";
                            }
                            else
                            {
                                msj.codigo = 333;
                                msj.Descripcion = "EL TEMA NO EXISTE";
                            }
                        }
                        else
                        {
                            msj.codigo = 333;
                            msj.Descripcion = "NO EXISTE EL ID TEMA EN TEMA-USUARIO";
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

        [HttpPost("compartirTema")]
        public async Task<ActionResult<respuestaAPIMiri>> Post(MCompartir _compartir)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var iduser = await _dbContext.Usuarios.Where(c => c.Usuario1 == _compartir._username).Select(c => c.IdUsuario).FirstOrDefaultAsync();
                    if (iduser <= 0 )
                    {
                        msj.codigo = -300;
                        msj.Descripcion = "ID USUARIO NO VALIDO";
                    }
                    else
                    {
                         
                        var existeUser = await _dbContext.Usuarios.FindAsync(iduser);
                        if (existeUser != null)
                        {
                            var existeTema = await _dbContext.CatTemas.FindAsync(_compartir._idDirectorio);
                            if(existeTema != null)
                            {
                                var compartido = await _dbContext.TemaUsuarios.Where(c => c.IdTema == _compartir._idDirectorio && c.IdUsuario == iduser).FirstOrDefaultAsync<TemaUsuario>();
                                if (compartido is null)
                                {
                                    var tu = new TemaUsuario
                                    {
                                        IdTema = _compartir._idDirectorio,
                                        IdUsuario = iduser
                                    };
                                    _dbContext.TemaUsuarios.Add(tu);
                                    await _dbContext.SaveChangesAsync();
                                    //Obtener todas las clasificaciones que pertencen a tema
                                    var ListidCT = await (from cc in _dbContext.CatClasificacions
                                                          join ct in _dbContext.ClasificacionTemas on cc.IdClasificacion equals ct.IdClasificacion
                                                          join t in _dbContext.CatTemas on ct.IdTema equals t.IdTema
                                                          where t.IdTema == _compartir._idDirectorio
                                                          select new ClasificacionTema
                                                          {
                                                              IdCt = ct.IdCt,
                                                              IdClasificacion = ct.IdClasificacion
                                                          }).ToListAsync();
                                    foreach (var item in ListidCT)
                                    {
                                        //Verificar que el usuario y clasifición existen en la tabla UsuariosCT
                                        var existeUsuarioCT = await _dbContext.UsuariosCts.Where(c => c.IdUsuario == iduser && c.IdCt == item.IdCt).FirstOrDefaultAsync();
                                        if (existeUsuarioCT == null)
                                        {
                                            var uct = new UsuariosCt
                                            {
                                                IdUsuario = iduser,
                                                IdCt = item.IdCt
                                            };
                                            _dbContext.UsuariosCts.Add(uct);
                                        }
                                        


                                        //Obtener todos los grupos que pertenecen a cada clasificación
                                        var ListidGCT = await (from g in _dbContext.CatGrupos
                                                               join gct in _dbContext.GrupoClasificacionTemas on g.IdGrupo equals gct.IdGrupo
                                                               join ct in _dbContext.ClasificacionTemas on gct.IdCt equals ct.IdCt
                                                               where ct.IdClasificacion == item.IdClasificacion
                                                               select new GrupoClasificacionTema
                                                               {
                                                                   IdGct = gct.IdGct
                                                               }).ToListAsync();
                                        foreach (var item2 in ListidGCT)
                                        {
                                            //Verificar que el usuario y grupo existen en la tabla UsuariosGCT
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

                                    }
                                    await _dbContext.SaveChangesAsync();
                                    dbContextTransaction.Commit();
                                    msj.codigo = 111;
                                    msj.Descripcion = "TEMA COMPARTIDO CON EXITO";
                                }
                                else
                                {
                                    msj.codigo = 222;
                                    msj.Descripcion = "YA SE COMPARTIÓ EL TEMA CON ESTE USUARIO";
                                }
                            }
                            else
                            {
                                msj.codigo = 333;
                                msj.Descripcion = "ID TEMA NO EXISTE";
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
        [HttpDelete("quitarAccesoTema/{idDirectorio}/{userName}")]
        public async Task<ActionResult<respuestaAPIMiri>> DeleteQuitarAccesoTema(int idDirectorio, string userName)
        {
            ClasificacionController _clasifcontroller = new ClasificacionController(_dbContext);
            //Obtener todas las clasificaciones que pertencen a tema
            //Este codigo lo pongo afuera de la transacción porque el metodo DeleteGrupo tiene otra transacción y marca error al contener una transaccion en otra.
            var ListidClasif = await (from cc in _dbContext.CatClasificacions
                                      join ct in _dbContext.ClasificacionTemas on cc.IdClasificacion equals ct.IdClasificacion
                                      join t in _dbContext.CatTemas on ct.IdTema equals t.IdTema
                                      where t.IdTema == idDirectorio
                                      select new ClasificacionTema
                                      {
                                          IdClasificacion = ct.IdClasificacion
                                      }).ToListAsync();
            if (ListidClasif.Count > 0)
            {
                //Aqui elimino todos las clasificaciones que pertenecen a TEMA
                foreach (var item in ListidClasif)
                {
                    await _clasifcontroller.DeleteAquitarAccesoClasificacion(item.IdClasificacion,userName);
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
                            var existeTema = await _dbContext.CatTemas.FindAsync(idDirectorio);
                            if (existeTema != null)
                            {
                                var deleteTemaUsuario = await _dbContext.TemaUsuarios.Where(c => c.IdTema == idDirectorio && c.IdUsuario == iduser).ToListAsync<TemaUsuario>();
                                if (deleteTemaUsuario != null)
                                {
                                    _dbContext.TemaUsuarios.RemoveRange(deleteTemaUsuario);
                                    await _dbContext.SaveChangesAsync();
                                    dbContextTransaction.Commit();
                                    msj.codigo = 666;
                                    msj.Descripcion = "SE DEJO DE COMPARTIR EL DIRECTORIO";
                                }
                            }
                            else
                            {
                                msj.codigo = 333;
                                msj.Descripcion = "ID TEMA NO EXISTE";
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
