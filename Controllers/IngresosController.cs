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

        public override async Task<IActionResult> Create(IngresoDto dto)
        {
            
            foreach (var detalle in dto.Detalle)
            {
                // verificamos si existe edicion
                var edicion = await _context.Ediciones
                .Where(e => e.IdArticulo == detalle.IdArticulo && e.NroEdicion == detalle.NroEdicion && e.FechaEdicion == detalle.FechaEdicion)
                .SingleOrDefaultAsync();

                if (edicion == null || !edicion.Activo)
                {
                    // generamos una nueva edicion
                    Edicion nuevaEdicion = _mapper.Map<Edicion>(detalle);
                    await _context.Ediciones.AddAsync(nuevaEdicion);
                    await _context.SaveChangesAsync();
                    //asignamos idEdicion al detalle
                    detalle.IdEdicion = nuevaEdicion.Id;
                }
                else
                {
                    // ya existe la edicion, actualizamos
                    edicion.CantidadInicial += detalle.Cantidad;
                    edicion.CantidadActual += detalle.Cantidad;
                    _context.Ediciones.Update(edicion);
                    await _context.SaveChangesAsync();
                    //asignamos idEdicion al detalle
                    detalle.IdEdicion = edicion.Id;
                }
            }

            return await base.Create(dto);
        }

        protected override async Task ExecutePostSave(IngresoDto dto)
        {}

        [HttpPut("[action]/{id}")]
        public override async Task<IActionResult> Desactivar(long id)
        {
            var ingreso = await _context.Ingresos
            .Include(i => i.Detalle)
            .Where(i => i.Id == id)
            .SingleOrDefaultAsync();

            if (ingreso == null) return NotFound();

            foreach (var detalle in ingreso.Detalle)
            {
                // obtenemos la edicion
                var edicion = await _context.Ediciones
                .FindAsync(detalle.IdEdicion);
                
                // restamos el stock
                edicion.CantidadActual -= detalle.Cantidad;
                edicion.CantidadInicial -= detalle.Cantidad;

                if (edicion.CantidadInicial == 0)
                { // fue el ultimo ingreso anulado o el unico ingreso
                    edicion.Activo = false;
                    _context.Ediciones.Update(edicion);
                    await _context.SaveChangesAsync();
                }
            }
            
            ingreso.Activo = false;
            if (IsAuditEntity)
            {
                ingreso.FechaUltimaModificacion = DateTime.Now;
            }

            _context.Entry(ingreso).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
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
        public long? IdArticulo { get; set; }
        [Requerido]
        public long? IdPrecio { get; set; }
        public long? IdEdicion { get; set; } = null;
        [Requerido]
        [NoNegativo]
        public long Cantidad { get; set; }
        [Requerido]
        public DateTime? FechaEdicion { get; set; }
        [Requerido]
        public long? NroEdicion { get; set; }

    }

}