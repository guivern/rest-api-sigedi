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
                if (ediciones.Any(e => e.NroEdicion == detalleDto.NroEdicion) &&
                ediciones.Any(e => e.FechaEdicion == detalleDto.FechaEdicion) &&
                ediciones.Any(e => e.IdArticulo == detalleDto.IdArticulo))
                {
                    // ya existe, actualizamos
                    Edicion edicion = await _context.Ediciones.SingleOrDefaultAsync(e => e.NroEdicion == detalleDto.NroEdicion 
                    && e.FechaEdicion == detalleDto.FechaEdicion
                    && e.IdArticulo == detalleDto.IdArticulo);
                    
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
      
       [HttpPut("[action]/{id}")]
        public override async Task<IActionResult> Desactivar(long id)
        {
            if (IsSoftDelete)
            {
                var ingreso = await _context.Ingresos.FindAsync(id);

                var ingresoDetalle = await _context.IngresoDetalles.Where(a => a.IdIngreso == id).ToListAsync();
                
                if (ingreso == null) 
                {
                    return NotFound();
                }
                else{
                    //listamos las ediciones
                    var ediciones = await _context.Ediciones.ToListAsync();

                    //recorremos el detalle
                    foreach (var detalleIngreso in ingresoDetalle)
                    {
                        if (ediciones.Any(e => e.NroEdicion == detalleIngreso.NroEdicion) &&
                        ediciones.Any(e => e.FechaEdicion == detalleIngreso.FechaEdicion) &&
                        ediciones.Any(e => e.CantidadInicial <= detalleIngreso.Cantidad) &&
                        ediciones.Any(e => e.IdArticulo == detalleIngreso.IdArticulo))
                        {
                            Edicion edicion = await _context.Ediciones.SingleOrDefaultAsync(e => e.NroEdicion == detalleIngreso.NroEdicion 
                            && e.FechaEdicion == detalleIngreso.FechaEdicion
                            && e.IdArticulo == detalleIngreso.IdArticulo);
                            _context.Remove(edicion);
                            await _context.SaveChangesAsync();
                           
                        }
                        else{
                            
                            //actualizamos
                            Edicion edicion = await _context.Ediciones.SingleOrDefaultAsync(e => e.NroEdicion == detalleIngreso.NroEdicion 
                            && e.FechaEdicion == detalleIngreso.FechaEdicion
                            && e.IdArticulo == detalleIngreso.IdArticulo);

                            edicion.CantidadInicial -= detalleIngreso.Cantidad;
                            edicion.CantidadActual -= detalleIngreso.Cantidad;
                            _context.Ediciones.Update(edicion);
                            await _context.SaveChangesAsync();
                        }
                    }
                    ingreso.Activo = false;
                }
                if (IsAuditEntity)
                {
                    ingreso.FechaUltimaModificacion = DateTime.Now;
                }

                _context.Entry(ingreso).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return StatusCode(405);
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