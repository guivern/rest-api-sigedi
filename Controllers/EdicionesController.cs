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
    public class EdicionesController : CrudControllerBase<Edicion, EdicionDto>
    {
         public EdicionesController(DataContext context, IMapper mapper) 
        : base(context, mapper)
        {}

        protected override IQueryable<Edicion> IncludeListFields(IQueryable<Edicion> query)
        {
            return query
            .Include(e => e.Articulo)
            .Include(e => e.Precio)
            .Where(e => !e.Anulado); // no incluimos los anulados
        }

        [HttpPost]
        public async Task<IActionResult> DarBaja(EdicionDto dto)
        { 
            // obtenemos la edicion
            var edicion = await _context.Ediciones
            .FindAsync(dto.Id);

            if (edicion == null) return NotFound();
            
        
            //obtenemos la lista de los detalles que tengan esa edicion
            var ingresoDet = await _context.IngresoDetalles
                .Include(e => e.Edicion)
                .Where(e => e.IdEdicion == dto.Id && e.Anulable == true)
            .ToListAsync();
            
            //recorremos la lista de los detalles
            foreach (var ingresoLista in ingresoDet)
            {
                //obtenemos el ingreso
                var ingreso = await _context.Ingresos
                .Include(d => d.Detalle)
                .Where(i => i.Id == ingresoLista.IdIngreso)
                .SingleOrDefaultAsync();
      
                ingreso.Anulable = false;
                _context.Ingresos.Update(ingreso);
                await _context.SaveChangesAsync();

                //cambiamos en los detalles ingreso los estados
                ingresoLista.Anulable = false;
                ingresoLista.Editable = false;

                 _context.IngresoDetalles.Update(ingresoLista);
                await _context.SaveChangesAsync();
            }


            //creamos un egreso
            var nuevoEgreso = new Egreso();
            nuevoEgreso.IdEdicion = edicion.Id;
            nuevoEgreso.Cantidad = edicion.CantidadActual;
            nuevoEgreso.Fecha = DateTime.Now;
            nuevoEgreso.FechaCreacion = DateTime.Now;
            nuevoEgreso.IdUsuarioCreador = dto.IdUsuarioCreador;
            
            await _context.Egresos.AddAsync(nuevoEgreso);
            await _context.SaveChangesAsync();
            
            //cambiamos de estado y ceramos
            edicion.Activo = false;
            edicion.CantidadActual = 0;
            edicion.CantidadInicial = 0;

            _context.Ediciones.Update(edicion);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

    }

    public class EdicionDto : DtoBase
    {
        public long IdUsuarioCreador {get; set;}
    }
}