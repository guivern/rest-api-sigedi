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
    public class IngresosController : CrudControllerConDetalle<Ingreso, IngresoDto, IngresoDetalle, IngresoDetalleDto>
    {
        public IngresosController(DataContext context, IMapper mapper) : base(context, mapper)
        { }
        protected override async Task<bool> IsValidModel(IngresoDto dto)
        {
            if (dto.Detalle.Count == 0)
            {
                ModelState.AddModelError(
                nameof(dto.Detalle), "Debe ingresar al menos un detalle de ingreso");
                return false;
            }
            return true;
        }
        protected override IQueryable<Ingreso> IncludeListFields(IQueryable<Ingreso> query)
        {
            return query
            .Include(i => i.Proveedor)
            .Include(i => i.UsuarioCreador);
        }
        protected override IQueryable<Ingreso> IncludeDetailFields(IQueryable<Ingreso> query)
        {
            return query
                .Include(a => a.Detalle)
                    .ThenInclude(d => d.Articulo)
                .Include(a => a.Detalle)
                    .ThenInclude(d => d.Precio);
        }

        protected override async Task ExecutePostSave(IngresoDto dto)
        { 
            var ediciones = await _context.Ediciones.ToListAsync();
            // verificar si existe un nro edicion y fecha 
            foreach (var detalleDto in dto.Detalle)
            {
                if ((ediciones.Any(e => e.NroEdicion == detalleDto.NroEdicion)) &&
                ediciones.Any(e => e.FechaEdicion == detalleDto.FechaEdicion))
                {
                    // ya existe, actualizamos
                    Edicion edicion = await _context.Ediciones.SingleOrDefaultAsync(e => e.NroEdicion == detalleDto.NroEdicion);
                    edicion.CantidadInicial += detalleDto.Cantidad;
                    edicion.CantidadActual += detalleDto.Cantidad;
                    _context.Ediciones.Update(edicion);
                    await _context.SaveChangesAsync();
                }
                else{
                    // creamos
                    Edicion edicion = _mapper.Map<Edicion>(detalleDto);
                    await _context.Ediciones.AddAsync(edicion);
                    await _context.SaveChangesAsync();
                }
            } 
        }
    }

    public class IngresoDto : DtoConDetalle<IngresoDetalleDto>
    {
        [Requerido]
        public long? IdProveedor { get; set; }
        [Requerido]
        public long? IdUsuarioCreador { get; set; }
        public string TipoComprobante { get; set; }
        public string NumeroComprobante { get; set; }
    }

    public class IngresoDetalleDto : DtoBase
    {
        [Requerido]
        public long? IdArticulo {get; set;}
        [Requerido]
        public long? IdPrecio {get; set;}
        [Requerido]
        [NoNegativo]
        public long Cantidad { get; set; }
        [Requerido]
        public DateTime? FechaEdicion { get; set; }
        [Requerido]
        public long? NroEdicion { get; set; }
        
    }

}