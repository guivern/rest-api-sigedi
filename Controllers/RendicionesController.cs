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
    public class RendicionesController : CrudControllerConDetalle<Rendicion, RendicionDto, RendicionDetalle, RendicionDetalleDto>
    {
        public RendicionesController(DataContext context, IMapper mapper) : base(context, mapper)
        { }

        protected override async Task<bool> IsValidModel(RendicionDto dto)
        {
            //Debe contener al menos un detalle
            if (dto.Detalle.Count == 0)
            {
                ModelState.AddModelError(
                nameof(dto.Detalle), "Debe ingresar al menos un detalle");
                return ModelState.IsValid;
            }
            
            var index = 0;
            foreach (var detalle in dto.Detalle)
            {
                var distribucionDet =  await _context.DistribucionDetalles
                .Where(d => d.Id == detalle.IdDistribucionDetalle)
                .SingleOrDefaultAsync();
              
                //Devolucion no puede ser mayor a la cantidad distribuida
                if(detalle.Devoluciones > distribucionDet.Cantidad)
                {
                    ModelState.AddModelError(
                    $"Detalle[{index}].Devoluciones", "La cantidad devuelta supera a la cantidad distribuida");
                }

                //Si la cantidad que se quiere devolver supera en costo a lo que se quiera pagar
                var edicion = await _context.Ediciones
                .Include(e => e.Precio)
                .Where(e => e.Id == distribucionDet.IdEdicion)
                .SingleOrDefaultAsync();
                
                var costoDevuelto = detalle.Devoluciones * edicion.Precio.PrecioRendVendedor;
                var saldoModif = distribucionDet.Saldo - costoDevuelto;

                if(saldoModif < 0)
                {
                    ModelState.AddModelError(
                    $"Detalle[{index}].Devoluciones", "La cantidad devuelta supera en costo al saldo pendiente");
                }
                //Importe debe ser menor o igual que el saldo
                if(saldoModif < detalle.Importe && saldoModif > 0)
                {
                    ModelState.AddModelError(
                    $"Detalle[{index}].Importe", "El importe supera al saldo");
                }
                index++;
            }
           return ModelState.IsValid;
        }

        protected override IQueryable<Rendicion> IncludeListFields(IQueryable<Rendicion> query)
        {
            return query
            .Include(r => r.Vendedor)
            .Include(r => r.UsuarioCreador);
        }

        protected override IQueryable<Rendicion> IncludeDetailFields(IQueryable<Rendicion> query)
        {
            return query
                .Include(r => r.Detalle)
                    .ThenInclude(e => e.DistribucionDetalle)
                .Include(r => r.Detalle)
                    .ThenInclude(e => e.DistribucionDetalle.Edicion)
                .Include(r => r.Detalle)
                    .ThenInclude(e => e.DistribucionDetalle.Edicion.Articulo)
                .Include(r => r.Detalle)
                    .ThenInclude(e => e.DistribucionDetalle.Edicion.Precio);
        }

        protected override async Task ExecutePostSave(RendicionDto dto)
        {
            

            foreach (var detalle in dto.Detalle)
            {
                //Traemos el detalle de la distribucion correspondiente 
                var distribucionDet = await _context.DistribucionDetalles
                .Where(r => r.Id == detalle.IdDistribucionDetalle)
                .SingleOrDefaultAsync();

                //Traemos la distribucion
                var distribucion = await _context.Distribuciones
                .Where(d => d.Id == distribucionDet.IdDistribucion)
                .SingleOrDefaultAsync();

                //Una vez que se haga una rendicion la distribucion no se puede anular
                distribucion.Anulable = false;
                distribucion.Editable = false;
                _context.Distribuciones.Update(distribucion);
                await _context.SaveChangesAsync();
                

                distribucionDet.Anulable = false;
                distribucionDet.Editable = false;
                distribucionDet.YaSeDevolvio = true;

                if(detalle.Devoluciones >0)
                {
                    //si hubo devoluciones

                    //Edicion a reponer
                    var edicion = await _context.Ediciones
                    .Include(e => e.Precio)
                    .Where(e => e.Id == distribucionDet.IdEdicion)
                    .SingleOrDefaultAsync();

                    //Se actualizan los campos en distribuciones, segun la cantidad devuelta y pagada
                    distribucionDet.Saldo -= (decimal) detalle.Devoluciones * edicion.Precio.PrecioRendVendedor;
                    distribucionDet.Monto -= (decimal) detalle.Devoluciones * edicion.Precio.PrecioRendVendedor;
                    distribucionDet.Importe += (decimal) detalle.Importe;
                    distribucionDet.Saldo -= (decimal) detalle.Importe;
                    distribucionDet.Devoluciones += detalle.Devoluciones;
                    _context.DistribucionDetalles.Update(distribucionDet);
                    await _context.SaveChangesAsync();

                    //Se actualiza  stock con cantidad devuelta
                    edicion.CantidadActual += (long)detalle.Devoluciones;
                    edicion.Activo = true;
                    edicion.Anulado = false;
                    _context.Ediciones.Update(edicion);
                    await _context.SaveChangesAsync();

                }else{
                    //si no hubo devoluciones, solo se actualizan importe y saldo

                    distribucionDet.Importe += detalle.Importe;
                    distribucionDet.Saldo -= (decimal) detalle.Importe;
                    distribucionDet.Devoluciones += detalle.Devoluciones;
                    _context.DistribucionDetalles.Update(distribucionDet);
                    await _context.SaveChangesAsync();
                }
                
                //Si se cancela el saldo
                if(distribucionDet.Saldo == 0)
                {
                    distribucionDet.Activo = false;
                    _context.DistribucionDetalles.Update(distribucionDet);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public override async Task<IActionResult> Desactivar(long id)
        {
            var rendicion = await _context.Rendiciones
            .Include(d => d.Detalle)
            .SingleOrDefaultAsync(d => d.Id == id);

            if(rendicion == null) return NotFound();
            if(!rendicion.Anulable) return BadRequest();
            
            // se anula la rendicion, cambiamos estado
            //rendicion.Activo = false;
            rendicion.Anulado = true;
            rendicion.Anulable = false;
            _context.Rendiciones.Update(rendicion);
            await _context.SaveChangesAsync();
            
            foreach(var detalleRen in rendicion.Detalle){

                //traemos distribucionDetalle correspondiente
                var distribucionDet = await _context.DistribucionDetalles
                .SingleOrDefaultAsync(d => d.Id == detalleRen.IdDistribucionDetalle);

                //cambiar estado
                //detalleRen.Activo = false;
                detalleRen.Anulado = true;
                detalleRen.Anulable = false;

                //cuando el importe en la distribucion llegue a 0 y no tenga devoluciones se resetea
                if(distribucionDet.Importe == detalleRen.Importe &&
                distribucionDet.Devoluciones == detalleRen.Devoluciones){

                    distribucionDet.YaSeDevolvio = false;
                    distribucionDet.Editable = true;
                    distribucionDet.Anulable = true;
                    
                    var distribucion = await _context.Distribuciones
                    .SingleOrDefaultAsync(d => d.Id == distribucionDet.IdDistribucion);

                    distribucion.Editable = true;
                    distribucion.Anulable = true;

                    _context.Distribuciones.Update(distribucion);
                    await _context.SaveChangesAsync();
                }
                if(detalleRen.Devoluciones > 0){
                    //si hubo rendiciones
                    distribucionDet.Devoluciones -= (long) detalleRen.Devoluciones;
                    
                    //traemos la edicion para recalcular el monto y la cantidad en edicion
                    //Edicion a reponer
                    var edicion = await _context.Ediciones
                    .Include(e => e.Precio)
                    .Where(e => e.Id == distribucionDet.IdEdicion)
                    .SingleOrDefaultAsync();

                    edicion.CantidadActual -= (long) detalleRen.Devoluciones;
                    distribucionDet.Saldo += (decimal) detalleRen.Devoluciones * edicion.Precio.PrecioRendVendedor;
                    distribucionDet.Monto += (decimal) detalleRen.Devoluciones * edicion.Precio.PrecioRendVendedor;
                    distribucionDet.Importe -= (decimal) detalleRen.Importe;
                    distribucionDet.Saldo += (decimal) detalleRen.Importe;
                    distribucionDet.Activo = true;
                    _context.Ediciones.Update(edicion);
                    await _context.SaveChangesAsync();

                }else{

                    //si no hubo devoluciones
                    distribucionDet.Saldo += detalleRen.Importe;
                    distribucionDet.Importe -= (decimal) detalleRen.Importe;
                    distribucionDet.Activo = true;
                }
                _context.DistribucionDetalles.Update(distribucionDet);
                await _context.SaveChangesAsync();

            }
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public class RendicionDto : DtoConDetalle<RendicionDetalleDto>
    {
        [Requerido]
        public long? IdVendedor { get; set; }
        [Requerido]
        public long? IdUsuarioCreador { get; set; }
        public long? IdUsuarioModificador { get; set; }
        public decimal? MontoTotal { get; set; } = 0;
        public decimal? ImporteTotal { get; set; } = 0;
        public decimal? SaldoTotal { get; set; } = 0;
        public bool? Anulable { get; set; } = true;
    }

    public class RendicionDetalleDto : DtoBase  
    {
        [Requerido]
        public long? IdDistribucionDetalle { get; set; }
        [NoNegativo]
        [Requerido]
        public long? Devoluciones {get; set;}
        public decimal? Monto { get; set; } 
        [Requerido]
        public decimal? Importe { get; set; }
        public decimal? Saldo { get; set; } 
        public bool? Anulable { get; set; } = true;
    }
}