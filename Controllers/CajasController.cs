using System;
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
    public class CajasController : CrudControllerSinDetalle<Caja, CajaDto>
    {
        public CajasController(DataContext context, IMapper mapper) : base(context, mapper)
        {}

        protected override IQueryable<Caja> IncludeListFields(IQueryable<Caja> query)
        {
            return query
            .Include(c => c.UsuarioCreador);
        }

        [HttpGet("cajero/{idCajero}")]
        public async Task<IActionResult> GetCajaActiva(long idCajero)
        {
            var caja = await _context.Cajas
            .Include(c => c.UsuarioCreador)
            .Where(c => c.IdUsuarioCreador == idCajero && c.Activo)
            .SingleOrDefaultAsync();

            if(caja == null) return NotFound();

            return Ok(caja);
        }

        public override async Task<IActionResult> Create(CajaDto dto)
        {
            // solo debe existir una caja activa por usuario
            // vamos a desactivar las cajas previamente activas del usuario
            var cajas = await _context.Cajas
            .Where(c => c.IdUsuarioCreador == dto.IdUsuarioCreador)
            .ToListAsync();

            foreach(var caja in cajas)
            {
                caja.Activo = false;
            }

            _context.UpdateRange(cajas);
            
            dto.Monto = dto.MontoInicial;
            return await base.Create(dto);
        }
    }

    public class CajaDto: DtoBase
    {
        [Requerido]
        public long? IdUsuarioCreador { get; set; }
        public DateTime? FechaCierre {get; set;}
        [Requerido] 
        [NoNegativo]
        public Decimal? MontoInicial {get; set;} = 0;
        public Decimal? Monto {get; set;} = 0;
    }
}