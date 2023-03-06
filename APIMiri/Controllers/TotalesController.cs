using Microsoft.AspNetCore.Mvc;
using APIMiri.Models;
using Microsoft.EntityFrameworkCore;
using APIMiri.Data;

namespace APIMiri.Controllers
{
    [ApiController]
    [Route("totalesController")]
    public class TotalesController : Controller
    {
        private readonly DbMiriContext _dbContext;

        public TotalesController(DbMiriContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("readTotales/{idUser}")]
        public async Task<ActionResult<Mtotales>> Get(int idUser)
        {
            Mtotales mtotales = new Mtotales();
            mtotales.totalTemas = _dbContext.TemaUsuarios.Where(c => c.IdUsuario == idUser).ToList().Count();
            mtotales.totalClasif = _dbContext.UsuariosCts.Where(c => c.IdUsuario == idUser).ToList().Count();
            mtotales.totalGrupos = _dbContext.UsuariosGcts.Where(c => c.IdUsuario == idUser).ToList().Count();
            mtotales.totalUsuarios = _dbContext.Usuarios.Count();
            return mtotales;
        }
    }
}
