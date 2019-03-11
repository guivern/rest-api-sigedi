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
    public class ProveedoresController : CrudControllerSinDetalle<Proveedor, ProveedorDto>
    {
        public ProveedoresController(DataContext context, IMapper mapper) : base(context, mapper)
        { }

        // evaluamos que no se repita registros del proveedor
        protected override async Task<bool> IsValidModel(ProveedorDto dto)
        {
            if (await _context.Proveedores
            .AnyAsync(v => v.RazonSocial == dto.RazonSocial && v.Id != dto.Id))
            {
                ModelState.AddModelError(
                nameof(dto.RazonSocial),
                $"Ya existe un proveedor con la misma Razón Social \"{dto.RazonSocial}\" en el sistema");
                return false;

            } else if (await _context.Proveedores
            .AnyAsync(v => v.NumeroDocumento == dto.NumeroDocumento && v.Id != dto.Id))
            {
                ModelState.AddModelError(
                nameof(dto.NumeroDocumento),
                $"Ya existe un proveedor con el mismo RUC \"{dto.NumeroDocumento}\" en el sistema");
                return false;
            }
            return true;
        }
    }
    public class ProveedorDto: DtoBase
    {
        [Requerido]
        public string RazonSocial { get; set; }
        [Requerido]
        public string NumeroDocumento { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "No es una dirección de correo válida")]
        public string Email { get; set; }
        public bool? Activo { get; set; } = true;
        public string DireccionWeb {get; set;}
        public string Contacto {get; set;}
        public string NumeroContacto {get; set;}
        public string Ciudad {get; set;}
        public string Barrio {get; set;}
    }
}