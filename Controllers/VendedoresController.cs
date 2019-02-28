using System;
using System.ComponentModel.DataAnnotations;
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
    public class VendedoresController : CrudControllerBase<Vendedor, VendedorDto>
    {
        public VendedoresController(DataContext context, IMapper mapper) : base(context, mapper)
        { }

        public override async Task<IActionResult> Create(VendedorDto dto)
        {
            // evaluamos si existe vendedor
            if (await _context.Vendedores
            .AnyAsync(v => v.NumeroDocumento == dto.NumeroDocumento))
            {
                ModelState.AddModelError(
                nameof(dto.NumeroDocumento),
                $"Ya existe un vendedor con número de documento \"{dto.NumeroDocumento}\" en el sistema");
                
                return BadRequest(ModelState);
            }
            // es un vendedor nuevo
            var vendedor = _mapper.Map<Vendedor>(dto);
            _context.Vendedores.Add(vendedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Detail", new { id = vendedor.Id }, vendedor);
        }

        public override async Task<IActionResult> Update(long id, VendedorDto dto)
        {
            if (id != dto.Id || dto.Id == null) return BadRequest();

            var vendedor = await _context.Vendedores.FindAsync(id);
            if (vendedor == null) return NotFound();

            if (await _context.Vendedores
            .AnyAsync(v => v.NumeroDocumento == dto.NumeroDocumento && v.Id != id))
            {
                ModelState.AddModelError(
                nameof(dto.NumeroDocumento),
                $"Ya existe un vendedor con número de documento \"{dto.NumeroDocumento}\" en el sistema");
                
                return BadRequest(ModelState);
            }
            
            vendedor = _mapper.Map<VendedorDto, Vendedor>(dto, vendedor);

            _context.Entry(vendedor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
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
        public bool? Activo { get; set; } = true;
    }
}