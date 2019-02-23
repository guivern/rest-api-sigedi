using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using rest_api_sigedi.Models;
using rest_api_sigedi.Annotations;

namespace rest_api_sigedi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto credenciales)
        {
            if (credenciales == null)
            {
                return BadRequest(new { Error = "Debe enviar credenciales" });
            }

            var usuario = await _context.Usuarios.Where(u => u.Activo).Include(u => u.Rol).FirstOrDefaultAsync(u => u.Username == credenciales.Username);

            if (usuario == null)
            {
                return Unauthorized(new { Error = "No existe el usuario" });
            }

            using (SHA512 shaEncrypter = new SHA512Managed())
            {
                var hashed = shaEncrypter.ComputeHash(
                    Encoding.UTF8.GetBytes(credenciales.Password)
                );

                if (!Enumerable.SequenceEqual(usuario.HashPassword, hashed))
                {
                    return Unauthorized(new { Error = "Verifique su contraseña" });
                }
            }
            return Ok(new { Token = GenerateJwtToken(usuario), Usuario = usuario });

        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre ),
                new Claim("idusuario", usuario.Id.ToString() ),
                new Claim("rol", usuario.Rol.Nombre ),
                new Claim("nombre", usuario.Nombre )
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));


            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtAudience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginDto
    {
        [Requerido]
        public string Username { get; set; }
        [Requerido]
        public string Password { get; set; }
    }
}