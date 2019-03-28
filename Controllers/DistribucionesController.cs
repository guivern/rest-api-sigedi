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
                nameof(dto.Detalle), "Debe ingresar al menos un detalle para la distribución");
                return false;
            }
            //verificamos que la cantidad ingresada no supere el stock
            foreach (var detalle in dto.Detalle)
            {
                var edicion = await _context.Ediciones
                    .Where(e => e.Id == detalle.IdEdicion)
                    .SingleOrDefaultAsync();

                if (detalle.Cantidad > edicion.CantidadActual)
                {
                    ModelState.AddModelError(
                    nameof(dto.Detalle), "La cantidad ingresada excede el stock");
                    return false;
                }
            }
            return true;
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

        public override async Task<IActionResult> Create(DistribucionDto dto)
        {
            if (await IsValidModel(dto))
            {
                foreach (var detalle in dto.Detalle)
                {
                    //verificamos que exista un movimiento
                    var movimiento = await _context.Movimientos
                    .Where(m => m.IdEdicion == detalle.IdEdicion && m.IdVendedor == dto.IdVendedor
                    && m.Activo == true).SingleOrDefaultAsync();

                    //obtenemos la edicion 
                    var edicion = await _context.Ediciones
                    .Include(p => p.Precio)
                    .Where(e => e.Id == detalle.IdEdicion)
                    .SingleOrDefaultAsync();

                    if (movimiento == null)
                    {
                        //generamos un movimiento nuevo
                        var nuevoMovimiento = new Movimiento();
                        nuevoMovimiento.IdVendedor = dto.IdVendedor;
                        nuevoMovimiento.IdEdicion = detalle.IdEdicion;
                        nuevoMovimiento.Llevo = detalle.Cantidad;
                        nuevoMovimiento.Monto = nuevoMovimiento.Llevo * edicion.Precio.PrecioRendVendedor;
                        nuevoMovimiento.Saldo = nuevoMovimiento.Llevo * edicion.Precio.PrecioRendVendedor;

                        await _context.Movimientos.AddAsync(nuevoMovimiento);
                        await _context.SaveChangesAsync();
                        //asignamos idMovimiento al detalle
                        detalle.IdMovimiento = nuevoMovimiento.Id;
                    }
                    else
                    {
                        //ya existe el movimiento, actualizamos
                        movimiento.Llevo += detalle.Cantidad;
                        movimiento.Monto += (detalle.Cantidad * edicion.Precio.PrecioRendVendedor );
                        movimiento.Saldo += (detalle.Cantidad * edicion.Precio.PrecioRendVendedor); 
                        _context.Movimientos.Update(movimiento);
                        await _context.SaveChangesAsync();
                        //asignamos idMovimiento al detalle
                        detalle.IdMovimiento = movimiento.Id;
                    }

                    //disminuimos del stock de esa edicion
                    edicion.CantidadActual -= detalle.Cantidad;
                    _context.Ediciones.Update(edicion);
                    await _context.SaveChangesAsync(); 
                }
                return await base.Create(dto);
            }
            return BadRequest(ModelState);
        }
    }



    public class DistribucionDto : DtoConDetalle<DistribucionDetalleDto>
    {
        [Requerido]
        public long IdVendedor { get; set; }
        [Requerido]
        public long? IdUsuarioCreador { get; set; }

    }
    public class DistribucionDetalleDto : DtoBase
    {
        [Requerido]
        public long IdEdicion { get; set; }
        public long? IdMovimiento {get; set;} = null;
        [Requerido]
        [NoNegativo]
        public long Cantidad { get; set; }
     
    }
}