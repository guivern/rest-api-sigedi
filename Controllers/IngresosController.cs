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

                if (edicion == null)
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
                    edicion.Activo = true;
                    edicion.Anulado = false;
                    _context.Ediciones.Update(edicion);
                    await _context.SaveChangesAsync();
                    //asignamos idEdicion al detalle
                    detalle.IdEdicion = edicion.Id;
                }
            }

            return await base.Create(dto);
        }

        [HttpPut("{id}")]
        public override async Task<IActionResult> Update(long id, IngresoDto dto)
        {
            
            foreach (var detalleDto in dto.Detalle){

                // obtenemos la edicion del detalle
                var edicion = await _context.Ediciones
                .Where(e => e.IdArticulo == detalleDto.IdArticulo && e.NroEdicion == detalleDto.NroEdicion && e.FechaEdicion == detalleDto.FechaEdicion)
                .SingleOrDefaultAsync();
                
                
                //actualizamos stock
                if (detalleDto.Id == null)
                {   
                    if(edicion == null){
                        // generamos una nueva edicion
                        Edicion nuevaEdicion = _mapper.Map<Edicion>(detalleDto);
                        await _context.Ediciones.AddAsync(nuevaEdicion);
                        await _context.SaveChangesAsync();
                        //asignamos idEdicion al detalle
                        detalleDto.IdEdicion = nuevaEdicion.Id;
                    }
                    else{
                        
                        edicion.CantidadActual += (long)detalleDto.Cantidad;
                        edicion.CantidadInicial += (long)detalleDto.Cantidad;
                        edicion.Activo = true;
                        edicion.Anulado = false;
                        _context.Ediciones.Update(edicion);
                        await _context.SaveChangesAsync();
                        detalleDto.IdEdicion = edicion.Id;
                    }   
                     
                }
                else
                {   // es un detalle existente
                    // obtenemos el detalle
                    var detalleDb = await _context.IngresoDetalles.FindAsync(detalleDto.Id);
                    //actualizamos stock
                    edicion.CantidadActual += (long)detalleDto.Cantidad - detalleDb.Cantidad;
                    edicion.CantidadInicial += (long)detalleDto.Cantidad - detalleDb.Cantidad;
                    //actualizamos precio en la edicion
                    edicion.IdPrecio = (long)detalleDto.IdPrecio;
                    _context.Ediciones.Update(edicion);
                    await _context.SaveChangesAsync();
                    //actualizamos precio en los ingresos anteriores de la misma edicion
                    var ingresosAnteriores = await _context.IngresoDetalles.Where(i => i.IdEdicion == edicion.Id).ToListAsync();
                    foreach (var ingreso in ingresosAnteriores)
                    {
                        ingreso.IdPrecio = (long)detalleDto.IdPrecio;
                    }
                    _context.IngresoDetalles.UpdateRange(ingresosAnteriores);
                    await _context.SaveChangesAsync();
                }
                
            }
            return await base.Update(id,dto);
            
        }
        
        protected override async Task ExecuteBeforeSave(IngresoDto dto)
        {
            
            // verificamos los detalles eliminados para reponer stock
            if (dto.Id != null)
            {
                
                var detallesDb = await _context.IngresoDetalles
                .Where(d => d.IdIngreso == dto.Id)
                .ToListAsync();

                foreach (var detDb in detallesDb)
                {
                    var seElimina = true;
                    foreach (var detDto in dto.Detalle)
                    {
                        if (detDb.Id == detDto.Id)
                        {
                            seElimina = false;
                        }
                    }
                    if (seElimina)
                    {
                        var edicion = await _context.Ediciones
                        .SingleOrDefaultAsync(e => e.Id == detDb.IdEdicion);
                        edicion.CantidadActual -= detDb.Cantidad;
                        edicion.CantidadInicial -= detDb.Cantidad;
                        if(edicion.CantidadInicial == 0){
                            edicion.Activo = false;
                            edicion.Anulado = true;
                        }
                        _context.Ediciones.Update(edicion);
                        await _context.SaveChangesAsync();
                    }
                }

            }

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
            if(!ingreso.Anulable) return BadRequest();

            //se anula el ingreso
            ingreso.Anulado = true;
            ingreso.Editable = false;
            ingreso.Anulable = false;
            _context.Ingresos.Update(ingreso);

            foreach (var detalle in ingreso.Detalle)
            {
                // obtenemos la edicion
                var edicion = await _context.Ediciones
                .FindAsync(detalle.IdEdicion);
                
                // restamos el stock
                edicion.CantidadActual -= detalle.Cantidad;
                edicion.CantidadInicial -= detalle.Cantidad;

                //se anula el detalle
                detalle.Anulado = true;
                detalle.Editable = false;
                detalle.Anulable = false;
                _context.IngresoDetalles.Update(detalle);
                await _context.SaveChangesAsync();

                if (edicion.CantidadInicial == 0)
                { // fue el ultimo ingreso anulado o el unico ingreso
                    edicion.Activo = false;
                    edicion.Anulado = true;
                    _context.Ediciones.Update(edicion);
                    await _context.SaveChangesAsync();
                }
            }
            
            //ingreso.Activo = false;
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
        [Requerido]
        public string TipoComprobante { get; set; }
        [Requerido]
        public string NumeroComprobante { get; set; }
        public bool? Anulable { get; set; } = true;
        public bool? Editable { get; set; } = true;
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
        [MayorACero]
        public long Cantidad { get; set; }
        [Requerido]
        public DateTime? FechaEdicion { get; set; }
        [Requerido]
        public long? NroEdicion { get; set; }
        public bool? Anulable { get; set; } = true;
        public bool? Editable { get; set; } = true;

    }

}