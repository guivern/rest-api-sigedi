using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rest_api_sigedi.Annotations;
using rest_api_sigedi.Models;

namespace rest_api_sigedi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendedoresController : CrudControllerSinDetalle<Vendedor, VendedorDto>
    {
        public VendedoresController(DataContext context, IMapper mapper) : base(context, mapper)
        { }

        // evaluamos que no se repita un registro de vendedor
        protected override async Task<bool> IsValidModel(VendedorDto dto)
        {
            if (await _context.Vendedores
            .AnyAsync(v => v.NumeroDocumento == dto.NumeroDocumento && v.Id != dto.Id))
            {
                ModelState.AddModelError(
                nameof(dto.NumeroDocumento),
                $"Ya existe un vendedor con número de documento \"{dto.NumeroDocumento}\" en el sistema");

                return false;
            }
            return true;
        }

    }
    public class VendedorDto : DtoBase
    {
        [Requerido]
        public string Nombre { get; set; }
        [Requerido]
        public string Apellido { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        [Requerido]
        public string TipoDocumento { get; set; }
        [Requerido]
        public string NumeroDocumento { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        [EmailAddress(ErrorMessage = "No es una dirección de correo válida")]
        public string Email { get; set; }
        public string ZonaVenta {get; set;}
        public bool? Activo { get; set; } = true;
        //se agregan los campos nuevos 
        public string Nacionalidad {get; set;}
        public string TelefonoMovil {get; set;}
        public DateTime? FechaIngreso {get; set;}
        public string Descripcion {get; set;}
    }
}