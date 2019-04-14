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
    public class DistribucionesController : CrudControllerConDetalle<Distribucion, DistribucionDto, DistribucionDetalle, DistribucionDetalleDto>
    {
        public DistribucionesController(DataContext context, IMapper mapper) : base(context, mapper)
        { }

        protected override async Task<bool> IsValidModel(DistribucionDto dto)
        {
            if (dto.Detalle.Count == 0)
            {
                ModelState.AddModelError(
                nameof(dto.Detalle), "Debe ingresar al menos un detalle");
                return ModelState.IsValid;
            }

            //verificamos que la cantidad ingresada no supere el stock
            
            var index = 0;
            foreach (var detalle in dto.Detalle)
            {
                if(detalle.Id == null){
                    var edicion = await _context.Ediciones
                        .Where(e => e.Id == detalle.IdEdicion)
                        .SingleOrDefaultAsync();

                    if (detalle.Cantidad > edicion.CantidadActual)
                    {
                        ModelState.AddModelError(
                        $"Detalle[{index}].Cantidad", "Excede el stock");
                    }
                    index++;
                }else{
                    var edicion = await _context.Ediciones
                    .Where(e => e.Id == detalle.IdEdicion)
                    .SingleOrDefaultAsync();

                    var detalleDb = await _context.DistribucionDetalles
                        .Where(d => d.Id == detalle.Id)
                        .SingleOrDefaultAsync();

                    var cantidad =  detalle.Cantidad - detalleDb.Cantidad;
                    if(cantidad > edicion.CantidadActual){
                        ModelState.AddModelError(
                        $"Detalle[{index}].Cantidad", "Excede el stock");
                    }
                    index++;

                }
            }

            return ModelState.IsValid;
        }

        protected override IQueryable<Distribucion> IncludeListFields(IQueryable<Distribucion> query)
        {
            return query
            .Include(d => d.Vendedor)
            .Include(d => d.UsuarioCreador);
        }

        protected override IQueryable<Distribucion> IncludeDetailFields(IQueryable<Distribucion> query)
        {
            return query
                .Include(d => d.Detalle)
                    .ThenInclude(e => e.Edicion)
                .Include(d => d.Detalle)
                    .ThenInclude(e => e.Edicion.Articulo)
                .Include(d => d.Detalle)
                    .ThenInclude(e => e.Edicion.Precio);
        }

        protected override async Task ExecuteBeforeSave(DistribucionDto dto)
        {
            
            foreach (var detalleDto in dto.Detalle)
            {
                if(detalleDto.Editable == true){
                    
                    // obtenemos la edicion del detalle
                    var edicion = await _context.Ediciones
                    .Include(e => e.Precio)
                    .SingleOrDefaultAsync(e => e.Id == detalleDto.IdEdicion);

                    // calculamos monto y saldo 
                    detalleDto.Monto = detalleDto.Saldo = detalleDto.Cantidad * edicion.Precio.PrecioRendVendedor;

                    //actualizamos stock
                    if (detalleDto.Id == null)
                    {   // es un nuevo detalle
                        edicion.CantidadActual -= (long)detalleDto.Cantidad;
                    }
                    else
                    {   // es un detalle existente
                        // obtenemos el detalle
                        var detalleDb = await _context.DistribucionDetalles.FindAsync(detalleDto.Id);
                        edicion.CantidadActual -=  (long)detalleDto.Cantidad - detalleDb.Cantidad;
                    }

                    _context.Ediciones.Update(edicion);
                    await _context.SaveChangesAsync();
                }
            }
          
            // verificamos los detalles eliminados para reponer stock
            if (dto.Id != null)
            {
                var detallesDb = await _context.DistribucionDetalles
                .Where(d => d.IdDistribucion == dto.Id)
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
                        edicion.CantidadActual += detDb.Cantidad;
                        _context.Ediciones.Update(edicion);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            foreach (var detalleDto in dto.Detalle)
            {
                //obtenemos la lista de ingreso detalle con esa edicion
                var ingresoDet = await _context.IngresoDetalles
                .Include(e => e.Edicion)
                .Where(e => e.IdEdicion == detalleDto.IdEdicion && e.Anulable == true)
                .ToListAsync();

                //recorremos la lista de los detalles
                foreach (var ingresoLista in ingresoDet)
                {
                    //obtenemos el ingreso
                    var ingreso = await _context.Ingresos
                    .Include(d => d.Detalle)
                    .Where(i => i.Id == ingresoLista.IdIngreso)
                    .SingleOrDefaultAsync();

                    ingresoLista.Anulable = false;
                    ingresoLista.Editable = false;

                    _context.IngresoDetalles.Update(ingresoLista);
                    await _context.SaveChangesAsync();
                    
                    ingreso.Anulable = false;
                    

                    _context.Ingresos.Update(ingreso);
                    await _context.SaveChangesAsync();

                }
            }
        }
    
        public override async Task<IActionResult> Desactivar(long id)
        {
            var distribucion = await _context.Distribuciones
            .Include(d => d.Detalle)
            .SingleOrDefaultAsync(d => d.Id == id);

            if(distribucion == null) return NotFound();
            if(!distribucion.Anulable) return BadRequest();

            // se anula la distribucion
            distribucion.Anulado = true;
            distribucion.Editable = false;
            distribucion.Anulable = false;
            _context.Distribuciones.Update(distribucion);

            foreach(var d in distribucion.Detalle)
            {
                // se repone el stock correspondiente al detalle
                var edicion = await _context.Ediciones.SingleOrDefaultAsync(e => e.Id == d.IdEdicion);
                edicion.CantidadActual += d.Cantidad;
                _context.Ediciones.Update(edicion);
                
                //se anula el detalle
                d.Anulado = true;
                d.Editable = false;
                d.Anulable = false;
                _context.DistribucionDetalles.Update(d);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    
        [HttpGet("detalle/vendedor/{idVendedor}")]
        public async Task<IActionResult> GetDetalleByVendedor(long idVendedor)
        {
            var vendedor = await _context.Vendedores.FindAsync(idVendedor);
            if(vendedor == null) return NotFound();

            var distribuciones = _context.DistribucionDetalles
            .Include(d => d.Distribucion)
            .Include(d => d.Edicion).ThenInclude(e => e.Articulo)
            .Include(d => d.Edicion).ThenInclude(e => e.Precio)
            .Where(d => d.Distribucion.IdVendedor == idVendedor && d.Activo && !d.Anulado);

            return Ok(distribuciones);
        }
    }

    public class DistribucionDto : DtoConDetalle<DistribucionDetalleDto>
    {
        [Requerido]
        public long? IdVendedor { get; set; }
        [Requerido]
        public long? IdUsuarioCreador { get; set; }
        public long? IdUsuarioModificador { get; set; }
        public bool? Anulable { get; set; } = true;
        public bool? Editable { get; set; } = true;

    }

    public class DistribucionDetalleDto : DtoBase
    {
        [Requerido]
        public long? IdEdicion { get; set; }
        [Requerido]
        [MayorACero]
        public long? Cantidad { get; set; }
        public decimal? Monto { get; set; } // seteamos en el controller
        public decimal? Saldo { get; set; } // seteamos en el controller
        public bool? Anulable { get; set; } = true;
        public bool? Editable { get; set; } = true;
        public bool? YaSeDevolvio { get; set; } = false;
    }
}