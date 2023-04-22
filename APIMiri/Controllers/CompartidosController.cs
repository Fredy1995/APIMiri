using APIMiri.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using APIMiri.Data;

namespace APIMiri.Controllers
{
    [ApiController]
    [Route("compartidosController")]
    public class CompartidosController : Controller
    {
        private readonly DbMiriContext _dbContext;
        private respuestaAPIMiri msj = new respuestaAPIMiri();

        public CompartidosController(DbMiriContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("accesoAdirectorio/{idDirectorio}/{idUser}/{tipoD}")]
        public async Task<ActionResult<bool>> GetActivarBtn(int idDirectorio,int idUser,string tipoD)
        {
            bool respuesta = false;
            if (tipoD.Equals("t"))
            {
                var UserExistTema = await _dbContext.TemaUsuarios.Where(c => c.IdTema == idDirectorio && c.IdUsuario == idUser).FirstOrDefaultAsync();
                if (UserExistTema == null)
                {
                    respuesta = false;
                }
                else
                {
                    respuesta = true;
                }

            }else if (tipoD.Equals("c"))
            {
                var obtenerIDCT = _dbContext.ClasificacionTemas.Where(c => c.IdClasificacion == idDirectorio).Select(c => c.IdCt).FirstOrDefault();
                var UserExistClasif = await _dbContext.UsuariosCts.Where(c => c.IdCt == obtenerIDCT && c.IdUsuario == idUser).FirstOrDefaultAsync();
                if(UserExistClasif == null)
                {
                    respuesta = false;
                }
                else
                {
                    respuesta = true;
                }
            }
            else if (tipoD.Equals("g")){
                var obtenerIDGCT = _dbContext.GrupoClasificacionTemas.Where(c => c.IdGrupo == idDirectorio).Select(c => c.IdGct).FirstOrDefault();
                var UserExistGrupo = await _dbContext.UsuariosGcts.Where(c => c.IdGct == obtenerIDGCT && c.IdUsuario == idUser).FirstOrDefaultAsync();
                if(UserExistGrupo == null)
                {
                    respuesta = false;
                }
                else
                {
                    respuesta = true;
                }
            }
            return respuesta;
        }

        [HttpGet("compartidosConmigo/{iduser}")]
        public async Task<ActionResult<List<MDirectoriosCompartidos>>> Getcompartidos(int iduser)
        {
            List<MDirectoriosCompartidos> mdc = new List<MDirectoriosCompartidos>();
            var temasShared = await (from ct in _dbContext.CatTemas
                               join tu in _dbContext.TemaUsuarios on ct.IdTema equals tu.IdTema
                               where tu.IdUsuario == iduser
                               select new MDirectoriosCompartidos
                               {
                                   IdDirectorio = ct.IdTema,
                                   NameDirectorio = ct.Tema
                               }).ToListAsync();

            var clasifShared = from c in _dbContext.CatClasificacions
                         join ct in _dbContext.ClasificacionTemas on c.IdClasificacion equals ct.IdClasificacion
                         join uct in _dbContext.UsuariosCts on ct.IdCt equals uct.IdCt
                         where uct.IdUsuario == iduser && !(
                         from t in _dbContext.CatTemas
                         join tu in _dbContext.TemaUsuarios on t.IdTema equals tu.IdTema
                         where tu.IdUsuario == iduser
                         select t.IdTema).Contains(ct.IdTema)
                         select c;

            var grupoShared = from g in _dbContext.CatGrupos
                              join gct in _dbContext.GrupoClasificacionTemas on g.IdGrupo equals gct.IdGrupo
                              join ugct in _dbContext.UsuariosGcts on gct.IdGct equals ugct.IdGct
                              join ct in _dbContext.ClasificacionTemas on gct.IdCt equals ct.IdCt
                              where ugct.IdUsuario == iduser && !(
                           from c in _dbContext.CatClasificacions
                           join ct in _dbContext.ClasificacionTemas on c.IdClasificacion equals ct.IdClasificacion
                           join uct in _dbContext.UsuariosCts on ct.IdCt equals uct.IdCt
                           where uct.IdUsuario == iduser && !(
                           from t in _dbContext.CatTemas
                           join tu in _dbContext.TemaUsuarios on t.IdTema equals tu.IdTema
                           where tu.IdUsuario == iduser
                           select t.IdTema).Contains(ct.IdTema)
                           select uct.IdCt
                              ).Contains(gct.IdCt) && !(
                         from t in _dbContext.CatTemas
                         join tu in _dbContext.TemaUsuarios on t.IdTema equals tu.IdTema
                         where tu.IdUsuario == iduser
                         select t.IdTema).Contains(ct.IdTema)
                              select g;





            foreach (var item in temasShared)
            {
                mdc.Add(new MDirectoriosCompartidos { IdDirectorio = item.IdDirectorio, NameDirectorio = item.NameDirectorio });
            }

            foreach (var item in clasifShared)
            {
                mdc.Add(new MDirectoriosCompartidos { IdDirectorio = item.IdClasificacion, NameDirectorio = item.Clasificacion });
            }

            foreach (var item in grupoShared)
            {
                mdc.Add(new MDirectoriosCompartidos { IdDirectorio = item.IdGrupo, NameDirectorio = item.Grupo });
            }
       


            return mdc;
        }

    }
}
