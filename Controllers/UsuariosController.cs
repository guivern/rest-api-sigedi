using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rest_api_sigedi.Annotations;
using rest_api_sigedi.Models;

namespace rest_api_sigedi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _context;

        public UsuariosController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable>> List([FromQuery] bool Inactivos)
        {
            if (Inactivos)
                return await _context.Usuarios.Include(u => u.Rol).ToListAsync();
            return await _context.Usuarios.Include(u => u.Rol).Where(u => u.Activo).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> Detail(
            [FromRoute] long id, [FromQuery] bool Inactivo)
        {
            if (Inactivo)
                return await _context.Usuarios.FindAsync(id);
            return await _context.Usuarios.Where(u => u.Activo).SingleOrDefaultAsync(u => u.Id == id);
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> Activar(long id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null) return NotFound();

            usuario.Activo = true;

            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> Desactivar(long id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null) return NotFound();

            usuario.Activo = false;

            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsuarioDto usDto)
        {
            
            if( await _context.Usuarios.AnyAsync(u => u.Username == usDto.Username))
            {
                return BadRequest(new {usernameError ="El username ya existe"});
            }
            if(usDto.Password1 == null) return BadRequest(new {password1Error ="Debe ingresar password"});
            if(usDto.Password2 == null) return BadRequest(new {password2Error ="Debe confirmar el password"});
            if(!usDto.Password1.Equals(usDto.Password2)) return BadRequest(new {matchError ="Las contraseñas no coinciden"});

            var usuario = new Usuario();

            usuario.Username = usDto.Username;
            usuario.HashPassword = GenerateHash(usDto.Password1);
            usuario.Nombre = usDto.Nombre;
            usuario.IdRol = (long) usDto.IdRol;
            usuario.Apellido = usDto.Apellido;
            usuario.Direccion = usDto.Direccion;
            usuario.Email = usDto.Email;
            usuario.FechaNacimiento = usDto.FechaNacimiento;
            usuario.Telefono = usDto.Telefono;
            usuario.NumeroDocumento = usDto.NumeroDocumento;
            usuario.Activo = true;

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return CreatedAtAction("Detail", new { id = usuario.Id }, usuario);   
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Create(long id, UsuarioDto usDto)
        {
            if(usDto.Id == null || usDto.Id != id) return BadRequest();
            
            if(await _context.Usuarios.AnyAsync(u => u.Username == usDto.Username && usDto.Id != id))
            {
                return BadRequest(new {Error ="El username ya existe"});
            }

            var usuario = await _context.Usuarios.FindAsync(id);

            if(usuario == null) return NotFound();

            if(usDto.Password1 != null || usDto.Password2 != null)
            {
                if(usDto.Password1 == null) return BadRequest(new {password1Error ="Debe ingresar password"});
                if(usDto.Password2 == null) return BadRequest(new {password2Error ="Debe confirmar el password"});
                if(!usDto.Password1.Equals(usDto.Password2)) return BadRequest(new {matchError ="Los passwords no coinciden"});
                usuario.HashPassword = GenerateHash(usDto.Password1);
            }

            usuario.Username = usDto.Username;
            usuario.Nombre = usDto.Nombre;
            usuario.Apellido = usDto.Apellido;
            usuario.IdRol = (long) usDto.IdRol;
            usuario.Direccion = usDto.Direccion;
            usuario.Email = usDto.Email;
            usuario.FechaNacimiento = usDto.FechaNacimiento;
            usuario.Telefono = usDto.Telefono;
            usuario.NumeroDocumento = usDto.NumeroDocumento;
            usuario.Activo = (bool) usDto.Activo;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return NoContent();   
        }


        private byte[] GenerateHash(string pass)
        {
            if (!string.IsNullOrEmpty(pass))
            {
                using (SHA512 shaEncrypter = new SHA512Managed())
                {
                    var hashed = shaEncrypter.ComputeHash(
                        Encoding.UTF8.GetBytes(pass));
                    return hashed;
                }
            }

            return null;
        }


    }

    public class UsuarioDto
    {
        public long? Id {get; set;}
        [Requerido]
        [LongMax(32)]
        public string Username {get; set;}
        public string Password1 {get; set;}
        public string Password2 {get; set;}
        [Requerido]
        public long? IdRol {get; set;}
        [Requerido]
        [LongMax(64)]
        public string Nombre {get; set;}
        [Requerido]
        [LongMax(64)]
        public string Apellido {get; set;}
        public string Direccion {get; set;}
        public string Telefono {get; set;}
        public string NumeroDocumento {get; set;}
        public DateTime? FechaNacimiento {get; set;}
        [EmailAddress(ErrorMessage="No es una dirección de correo válida")]
        public string Email {get; set;}
        public bool? Activo {get; set;} = true;
    }
}