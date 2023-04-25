using APIMiri.Data;
using Microsoft.AspNetCore.Mvc;
using APIMiri.Models;
using APIMiri.Models.db;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace APIMiri.Controllers
{
    [ApiController]
    [Route("directorioController")]
    public class DirectorioController : Controller
    {
        private readonly DbMiriContext _dbContext;
        public DirectorioController(DbMiriContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("readDetallesDirectorio/{nameDirectorio}")]
        public async Task<ActionResult<List<MDetallesDirectorio>>> GetDetalles(string nameDirectorio)
        {
            List<MDetallesDirectorio> mdetalles = new List<MDetallesDirectorio>();
            TemaController tc = new TemaController(_dbContext);
            ClasificacionController cc = new ClasificacionController(_dbContext);
            GrupoController gc = new GrupoController(_dbContext);
            var tema =  await tc.GetEsTema(nameDirectorio);
            var clasif = await cc.GetEsClasif(nameDirectorio);
            var grupo = await gc.GetEsGrupo(nameDirectorio);
            if (tema.Value)
            {
                    var query = await _dbContext.CatTemas.Where(c => c.Tema == nameDirectorio).ToListAsync();
                    foreach (var item in query)
                    {
                        mdetalles.Add(new MDetallesDirectorio { Directorio = item.Tema, Propietario = item.Propietario, FechaCreacion = item.FechaCreacion, ModificadoPor = item.ModificadoPor, FechaModificacion = item.FechaModificacion });
                    }

            }
            else if(clasif.Value)
            {
              
                    var query = await _dbContext.CatClasificacions.Where(c => c.Clasificacion == nameDirectorio).ToListAsync();
                    foreach (var item in query)
                    {
                        mdetalles.Add(new MDetallesDirectorio { Directorio = item.Clasificacion, Propietario = item.Propietario, FechaCreacion = item.FechaCreacion, ModificadoPor = item.ModificadoPor, FechaModificacion = item.FechaModificacion });
                    }
              
            }
            else if (grupo.Value)
            {
                var query = await _dbContext.CatGrupos.Where(c => c.Grupo == nameDirectorio).ToListAsync();
                foreach (var item in query)
                {
                    mdetalles.Add(new MDetallesDirectorio { Directorio = item.Grupo, Propietario = item.Propietario, FechaCreacion = item.FechaCreacion, ModificadoPor = item.ModificadoPor, FechaModificacion = item.FechaModificacion });
                }
            }
            
            return mdetalles;
        }
    }
}
