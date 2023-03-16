using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Linq;
using APIMiri.Models;
using APIMiri.Models.db;
using System.Text;
using APIMiri.Data;
using System.Security.Cryptography.Xml;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Reflection.Metadata.Ecma335;
using Azure.Core;

namespace APIMiri.Controllers
{

    [ApiController]
    [Route("loginController")]
    public class LoginController : Controller
    {
        private readonly DbMiriContext _dbContext;
        private Seguridad _security = new Seguridad();
        respuestaAPIMiri msj = new respuestaAPIMiri();
        public LoginController(DbMiriContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("login/{user}/{pass}")]
        public async Task<ActionResult<MLogin>> Get(string user, string pass)
        {
            MLogin login = new MLogin();
            byte[] c = Encoding.UTF8.GetBytes(pass); //La contraseña llega encriptada 
            string PassClient = "0x" + BitConverter.ToString(c).Replace("-", string.Empty);
            try
            {
                var existeUser = await _dbContext.Usuarios.Where(c => c.Usuario1 == user).FirstOrDefaultAsync<Usuario>();
                if (existeUser != null)
                {
                    string PassUser = "0x" + BitConverter.ToString(existeUser.Contraseña).Replace("-", string.Empty);
                    if ((bool)existeUser.Habilitado)
                    {
                        if (PassClient.Equals(PassUser))
                        {
                            login.idUser = existeUser.IdUsuario;
                            login.nombreUsuario = existeUser.Usuario1;
                            login.nombre = existeUser.Nombre;
                            login.aPaterno = existeUser.APaterno;
                            login.aMaterno = existeUser.AMaterno;
                            login.perfil = await _dbContext.CatPerfiles.Where(c => c.IdPerfil == existeUser.IdPerfil).Select(c => c.Perfil).FirstOrDefaultAsync();

                            msj.codigo = 110;
                            msj.Descripcion = "ACCESO CORRECTO";
                        }
                        else
                        {
                            msj.codigo = -300;
                            msj.Descripcion = "CONTRASEÑA INCORRECTA";
                        }
                    }
                    else
                    {
                        msj.codigo = 220;
                        msj.Descripcion = "USUARIO BLOQUEADO";
                    }

                }
                else
                {
                    msj.codigo = 333;
                    msj.Descripcion = "USUARIO NO EXISTE";
                }



            } catch (Exception ex)
            {
                msj.codigo = -200;
                msj.Descripcion = "PROBLEMAS CON EL SERVIDOR - Error: " + ex;
               
            }
            login.respuestaAPI = msj;
            return login;
        }
        [HttpGet("readPerfiles")]
        public async Task<ActionResult<List<MPerfiles>>> GetPerfiles()
        {
            var query = await (from p in _dbContext.CatPerfiles
                             
                               select new MPerfiles
                               {
                                  IdPerfil = p.IdPerfil,
                                  Perfil = p.Perfil
                               }).ToListAsync();

            return query;
        }
        [HttpGet("readUsuario/{idUser}")]
        public async Task<ActionResult<List<MUsuario>>> GetUsuario(int idUser)
        {
            var query = await (from u in _dbContext.Usuarios
                               where u.IdUsuario == idUser
                               select new MUsuario
                               {
                                   Usuario1 = u.Usuario1,
                                   Nombre = u.Nombre,
                                   APaterno = u.APaterno,
                                   AMaterno = u.AMaterno,
                                   IdPerfil = u.IdPerfil,
                                   Habilitado = u.Habilitado,
                               }).ToListAsync();
            return query;
        }
        [HttpGet("readUsuarios")]
        public async Task<ActionResult<List<MUsuario>>> GetUsuarios()
        {
            var query = await (from u in _dbContext.Usuarios
                               join p in _dbContext.CatPerfiles on u.IdPerfil equals p.IdPerfil
                               select new MUsuario
                               {
                                  IdUsuario = u.IdUsuario,
                                  Usuario1 = u.Usuario1,
                                  Nombre= u.Nombre,
                                  APaterno= u.APaterno,
                                  AMaterno= u.AMaterno,
                                  IdPerfil =u.IdPerfil,
                                  Perfil = p.Perfil,
                                  FechaIngreso = u.FechaIngreso,
                                  Habilitado = u.Habilitado,
                               }).ToListAsync();

            return query;
        }
        
        [HttpPost("createUser")]
        public async Task<ActionResult<respuestaAPIMiri>> Post(MUsuario _user)
        {
           
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userExit = _dbContext.Usuarios.Where(c => c.Usuario1 == _user.Usuario1).FirstOrDefault<Usuario>();
                    if (userExit is null)
                    {
                        var a = new Usuario
                        {
                            Usuario1 = _user.Usuario1,
                            Contraseña = Encoding.UTF8.GetBytes(_security.cifrar(_user.Contraseña)),
                            Nombre = _user.Nombre,
                            APaterno = _user.APaterno,
                            AMaterno = _user.AMaterno,
                            IdPerfil = _user.IdPerfil,
                            FechaIngreso = DateTime.Now,
                            Habilitado = true
                        };
                        _dbContext.Usuarios.Add(a);
                        await _dbContext.SaveChangesAsync();
                        dbContextTransaction.Commit();
                        msj.codigo = 111;
                        msj.Descripcion = "USUARIO AGREGADO CON EXITO";

                    }
                    else
                    {
                        msj.codigo = 222;
                        msj.Descripcion = "EL USUARIO YA EXISTE";
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


        [HttpPut("updateDatosUser")]
        public async Task<ActionResult<respuestaAPIMiri>> Put(MUsuario _user)
        {

            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var existeUser = await _dbContext.Usuarios.FindAsync(_user.IdUsuario);
                    if (existeUser != null)
                    {
                        if (_user.check == true)
                        {
                            existeUser.Contraseña = Encoding.UTF8.GetBytes(_security.cifrar(_user.Contraseña));
                        }
                        else
                        {
                            existeUser.Usuario1 = _user.Usuario1;
                            existeUser.Nombre = _user.Nombre;
                            existeUser.APaterno = _user.APaterno;
                            existeUser.AMaterno = _user.AMaterno;
                            existeUser.IdPerfil = _user.IdPerfil;
                        }
                       
                        await _dbContext.SaveChangesAsync();
                        dbContextTransaction.Commit();
                        msj.codigo = 444;
                        msj.Descripcion = "INFORMACIÓN DE USUARIO ACTUALIZADA CON EXITO";
                    }
                    else
                    {

                        msj.codigo = 333;
                        msj.Descripcion = "USUARIO NO EXISTE";
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
