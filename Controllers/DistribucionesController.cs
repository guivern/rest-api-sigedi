using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using rest_api_sigedi.Annotations;
using rest_api_sigedi.Models;
using rest_api_sigedi.Utils;
using WkWrap.Core;

namespace rest_api_sigedi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistribucionesController : CrudControllerConDetalle<Distribucion, DistribucionDto, DistribucionDetalle, DistribucionDetalleDto>
    {
        private readonly ViewRender _viewRender;
        private readonly IConfiguration _configuration;
        
        public DistribucionesController(DataContext context, IMapper mapper, ViewRender viewRender, IConfiguration configuration) : base(context, mapper)
        { 
            _viewRender = viewRender;
            _configuration = configuration;
        }

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

            var distribuciones = await _context.DistribucionDetalles
            .Include(d => d.Distribucion)
            .Include(d => d.Edicion).ThenInclude(e => e.Articulo)
            .Include(d => d.Edicion).ThenInclude(e => e.Precio)
            .Where(d => d.Distribucion.IdVendedor == idVendedor && d.Activo && !d.Anulado)
            .ToListAsync();

            return Ok(distribuciones);
        }
        
        [HttpGet("detalle/deudas")]
        public async Task<IActionResult> GetDeudas(){

            var deudas = await _context.DistribucionDetalles
            .Include(d => d.Distribucion)
            .Include(d => d.Distribucion).ThenInclude(v => v.Vendedor)
            .Include(d => d.Edicion).ThenInclude(e => e.Articulo)
            .Include(d => d.Edicion).ThenInclude(e => e.Precio)
            .Where(d =>d.Activo && !d.Anulado)
            .ToListAsync();

            return Ok(deudas);
        }

        [HttpGet("reporte/ventas/")]
        public async Task<IActionResult> GetReporteVentas([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin, [FromQuery] String tipo){

            if( tipo == "Vendedores"){
            
                IEnumerable<DistribucionFecha> distribucionV =
                await _context.Distribuciones
                .Where(d => d.FechaCreacion.Date >= fechaInicio.Date
                && d.FechaCreacion.Date <= fechaFin.Date
                && !d.Anulado && !d.Editable)
                .OrderBy(r => r.Id)
                .GroupBy(
                    c => new {
                        FechaCreacionP = c.FechaCreacion.Date,
                        
                    })
                    .Select(g => new DistribucionFecha(){
                        
                        FechaCreacion = g.Key.FechaCreacionP,
                        DistribucionesFecha = g.ToList()

                    })
                .ToListAsync();

                //VENTAS POR VENDEDOR

                IEnumerable<DistribucionDetalleAgrupado> distribucionesAgrupadas =
                await _context.DistribucionDetalles
                .Include(d => d.Distribucion)
                .ThenInclude(d => d.Vendedor)
                .Include(d => d.Edicion)
                .ThenInclude(e => e.Articulo)
                .Include(d => d.Edicion)
                .ThenInclude(e => e.Precio)
                .Where(d => d.Distribucion.FechaCreacion.Date >= fechaInicio.Date
                && d.Distribucion.FechaCreacion.Date <= fechaFin.Date
                && !d.Distribucion.Anulado && !d.Editable)
                .OrderBy(r => r.Distribucion.FechaCreacion)
                .GroupBy( //agrupamos distribuciones por articulo
                    c => new {
                        IdVendedorP = c.Distribucion.Vendedor.Id,
                        FechaCreacionP = c.Distribucion.FechaCreacion.Date,
                        
                    })
                    .Select(g => new DistribucionDetalleAgrupado(){
                        IdVendedor = g.Key.IdVendedorP,
                        FechaCreacion = g.Key.FechaCreacionP,
                        TotalMonto = g.Sum( x=> x.Monto),
                        TotalImporte = (decimal) g.Sum( x=> x.Importe), 
                        TotalSaldo = g.Sum( x=> x.Saldo),
                        Distribuciones = g.ToList()

                    })
                .ToListAsync();
                
                IEnumerable<DistribucionDetalleAgrupado> distribucionesAgrupadasAV =
                await _context.DistribucionDetalles
                .Include(d => d.Distribucion)
                .ThenInclude(d => d.Vendedor)
                .Include(d => d.Edicion)
                .ThenInclude(e => e.Articulo)
                .Include(d => d.Edicion)
                .ThenInclude(e => e.Precio)
                .Where(d => d.Distribucion.FechaCreacion.Date >= fechaInicio.Date
                && d.Distribucion.FechaCreacion.Date <= fechaFin.Date
                && !d.Distribucion.Anulado && !d.Editable)
                .OrderBy(r => r.Distribucion.FechaCreacion)
                .GroupBy( //agrupamos distribuciones por articulo
                    c => new {
                        IdArticuloP = c.Edicion.Articulo,
                        FechaCreacionP = c.Distribucion.FechaCreacion.Date,
                        IdEdicionP = c.Edicion,
                        IdVendedorP = c.Distribucion.Vendedor
                    })
                    .Select(g => new DistribucionDetalleAgrupado(){
                        IdArticulo = g.Key.IdArticuloP.Id,
                        FechaCreacion = g.Key.FechaCreacionP,
                        IdEdicion = g.Key.IdEdicionP.Id,
                        IdVendedor = g.Key.IdVendedorP.Id,
                        NombreArticulo = g.Key.IdArticuloP.Descripcion,
                        NroEdicion = g.Key.IdEdicionP.NroEdicion,
                        FechaEdicion = g.Key.IdEdicionP.FechaEdicion,
                        TotalCantidad = g.Sum( x => x.Cantidad),
                        TotalDevoluciones = g.Sum( x => x.Devoluciones),
                        TotalMonto = g.Sum( x=> x.Monto),
                        TotalImporte = (decimal) g.Sum( x=> x.Importe), 
                        TotalSaldo = g.Sum( x=> x.Saldo),
                        Distribuciones = g.ToList()

                    })
                .ToListAsync();

                // resumen general del reporte
                ResumenDistribuciones resumen = new ResumenDistribuciones()
                {
                    TotalDistribuciones = distribucionesAgrupadas.Sum(d => d.TotalMonto),
                    TotalIngresos = distribucionesAgrupadas.Sum(d => d.TotalImporte),
                    TotalDeudas = distribucionesAgrupadas.Sum(d => d.TotalSaldo),
                    FechaInicioResumen = fechaInicio,
                    FechaFinResumen = fechaFin
                };

                // enlazamos los querys a la vista
                var model = new Dictionary<string, object>
                {
                    ["DistribucionDetalleAgrupado"] = distribucionesAgrupadas,
                    ["Resumen"] = resumen,
                    ["DistribucionV"] = distribucionV,
                    ["DistribucionDetalleAgrupadoAV"] = distribucionesAgrupadasAV,
                    // si hay mas querys agregamos aqui
                };

                // esta parte va a ser igual en todos los reportes
                // lo unico que cambiaria el nombre de la vista en el RenderAsync() 
                // y el nombre del reporte en el File()
                var wkhtmltopdfpath = _configuration.GetSection("Reportes:WkBinPath").Get<string>();
                var html = await _viewRender.RenderAsync("reporte_ventas_vendedores", model);
                var wkhtmltopdf = new FileInfo(wkhtmltopdfpath);
                var converter = new HtmlToPdfConverter(wkhtmltopdf);
                var pdf = converter.ConvertToPdf(html);

                return File(pdf, MediaTypeNames.Application.Pdf,
                        $"Reporte Ventas Vendedor {DateTime.Now:yyyyMMdd-hhmmss}.pdf");

            }else if(tipo == "Diarias"){

                //VENTAS DIARIAS

                IEnumerable<DistribucionDetalleAgrupado> distribucionesAgrupadas =
                await _context.DistribucionDetalles
                .Include(d => d.Distribucion)
                .ThenInclude(d => d.Vendedor)
                .Include(d => d.Edicion)
                .ThenInclude(e => e.Articulo)
                .Include(d => d.Edicion)
                .ThenInclude(e => e.Precio)
                .Where(d => d.Distribucion.FechaCreacion.Date >= fechaInicio.Date
                && d.Distribucion.FechaCreacion.Date <= fechaFin.Date
                && !d.Distribucion.Anulado && !d.Editable)
                .OrderBy(r => r.Id)
                .GroupBy( //agrupamos distribuciones por fechaCreacion
                    r => r.Distribucion.FechaCreacion.Date,
                    r => r,
                    (key, g) => new DistribucionDetalleAgrupado{
                        FechaCreacion = key,
                        TotalMonto = g.Sum( x=> x.Monto),
                        TotalImporte = (decimal) g.Sum( x=> x.Importe), 
                        TotalSaldo = g.Sum( x=> x.Saldo),
                        Distribuciones = g.ToList()
                    })
                .ToListAsync();  

                IEnumerable<DistribucionDetalleAgrupado> distribucionesAgrupadasA =
                await _context.DistribucionDetalles
                .Include(d => d.Distribucion)
                .ThenInclude(d => d.Vendedor)
                .Include(d => d.Edicion)
                .ThenInclude(e => e.Articulo)
                .Include(d => d.Edicion)
                .ThenInclude(e => e.Precio)
                .Where(d => d.Distribucion.FechaCreacion.Date >= fechaInicio.Date
                && d.Distribucion.FechaCreacion.Date <= fechaFin.Date
                && !d.Distribucion.Anulado && !d.Editable)
                .OrderBy(r => r.Distribucion.FechaCreacion)
                .GroupBy( //agrupamos distribuciones por articulo
                    c => new {
                        IdArticuloP = c.Edicion.Articulo,
                        FechaCreacionP = c.Distribucion.FechaCreacion.Date,
                        IdEdicionP = c.Edicion
                    })
                    .Select(g => new DistribucionDetalleAgrupado(){
                        IdArticulo = g.Key.IdArticuloP.Id,
                        FechaCreacion = g.Key.FechaCreacionP,
                        IdEdicion = g.Key.IdEdicionP.Id,
                        NombreArticulo = g.Key.IdArticuloP.Descripcion,
                        NroEdicion = g.Key.IdEdicionP.NroEdicion,
                        FechaEdicion = g.Key.IdEdicionP.FechaEdicion,
                        TotalCantidad = g.Sum( x => x.Cantidad),
                        TotalDevoluciones = g.Sum( x => x.Devoluciones),
                        TotalMonto = g.Sum( x=> x.Monto),
                        TotalImporte = (decimal) g.Sum( x=> x.Importe), 
                        TotalSaldo = g.Sum( x=> x.Saldo),
                        Distribuciones = g.ToList()

                    })
                .ToListAsync();

                // resumen general del reporte
                ResumenDistribuciones resumen = new ResumenDistribuciones()
                {
                    TotalDistribuciones = distribucionesAgrupadas.Sum(d => d.TotalMonto),
                    TotalIngresos = distribucionesAgrupadas.Sum(d => d.TotalImporte),
                    TotalDeudas = distribucionesAgrupadas.Sum(d => d.TotalSaldo),
                    FechaInicioResumen = fechaInicio,
                    FechaFinResumen = fechaFin
                };

                // enlazamos los querys a la vista
                var model = new Dictionary<string, object>
                {
                    ["DistribucionDetalleAgrupado"] = distribucionesAgrupadas,
                    ["Resumen"] = resumen,
                    ["DistribucionDetalleAgrupadoA"] = distribucionesAgrupadasA,
                    // si hay mas querys agregamos aqui
                };

                // esta parte va a ser igual en todos los reportes
                // lo unico que cambiaria el nombre de la vista en el RenderAsync() 
                // y el nombre del reporte en el File()
                var wkhtmltopdfpath = _configuration.GetSection("Reportes:WkBinPath").Get<string>();
                var html = await _viewRender.RenderAsync("reporte_ventas_diarias", model);
                var wkhtmltopdf = new FileInfo(wkhtmltopdfpath);
                var converter = new HtmlToPdfConverter(wkhtmltopdf);
                var pdf = converter.ConvertToPdf(html);

                return File(pdf, MediaTypeNames.Application.Pdf,
                        $"Reporte Ventas Diarias {DateTime.Now:yyyyMMdd-hhmmss}.pdf");

            }else if(tipo == "Articulos"){

                //VENTAS POR ARTICULOS

                IEnumerable<DistribucionFecha> distribucionV =
                await _context.Distribuciones
                .Where(d => d.FechaCreacion.Date >= fechaInicio.Date
                && d.FechaCreacion.Date <= fechaFin.Date
                && !d.Anulado && !d.Editable)
                .OrderBy(r => r.Id)
                .GroupBy(
                    c => new {
                        FechaCreacionP = c.FechaCreacion.Date,
                        
                    })
                    .Select(g => new DistribucionFecha(){
                        
                        FechaCreacion = g.Key.FechaCreacionP,
                        DistribucionesFecha = g.ToList()

                    })
                .ToListAsync();
                
                IEnumerable<DistribucionDetalleAgrupado> distribucionesAgrupadas =
                await _context.DistribucionDetalles
                .Include(d => d.Distribucion)
                .ThenInclude(d => d.Vendedor)
                .Include(d => d.Edicion)
                .ThenInclude(e => e.Articulo)
                .Include(d => d.Edicion)
                .ThenInclude(e => e.Precio)
                .Where(d => d.Distribucion.FechaCreacion.Date >= fechaInicio.Date
                && d.Distribucion.FechaCreacion.Date <= fechaFin.Date
                && !d.Distribucion.Anulado && !d.Editable)
                .OrderBy(r => r.Distribucion.FechaCreacion)
                .GroupBy( //agrupamos distribuciones por articulo
                    c => new {
                        IdArticuloP = c.Edicion.Articulo.Id,
                        FechaCreacionP = c.Distribucion.FechaCreacion.Date,
                        IdEdicionP = c.Edicion.Id,
                    })
                    .Select(g => new DistribucionDetalleAgrupado(){
                        IdArticulo = g.Key.IdArticuloP,
                        FechaCreacion = g.Key.FechaCreacionP,
                        IdEdicion = g.Key.IdEdicionP,
                        TotalCantidad = g.Sum( x => x.Cantidad),
                        TotalDevoluciones = g.Sum( x => x.Devoluciones),
                        TotalMonto = g.Sum( x=> x.Monto),
                        TotalImporte = (decimal) g.Sum( x=> x.Importe), 
                        TotalSaldo = g.Sum( x=> x.Saldo),
                        Distribuciones = g.ToList()

                    })
                .ToListAsync();


                IEnumerable<DistribucionDetalleAgrupado> distribucionesAgrupadasVendedor =
                await _context.DistribucionDetalles
                .Include(d => d.Distribucion)
                .ThenInclude(d => d.Vendedor)
                .Include(d => d.Edicion)
                .ThenInclude(e => e.Articulo)
                .Include(d => d.Edicion)
                .ThenInclude(e => e.Precio)
                .Where(d => d.Distribucion.FechaCreacion.Date >= fechaInicio.Date
                && d.Distribucion.FechaCreacion.Date <= fechaFin.Date
                && !d.Distribucion.Anulado && !d.Editable)
                .OrderBy(r => r.Distribucion.FechaCreacion)
                .GroupBy( //agrupamos distribuciones por articulo
                    c => new {
                        IdArticuloP = c.Edicion.Articulo.Id,
                        FechaCreacionP = c.Distribucion.FechaCreacion.Date,
                        IdEdicionP = c.Edicion,
                        IdVendedorP = c.Distribucion.Vendedor,
    
                    })
                    .Select(g => new DistribucionDetalleAgrupado(){
                        IdArticulo = g.Key.IdArticuloP,
                        FechaCreacion = g.Key.FechaCreacionP,
                        IdEdicion = g.Key.IdEdicionP.Id,
                        IdVendedor = g.Key.IdVendedorP.Id,
                        NombreVendedor = g.Key.IdVendedorP.NombreCompleto,
                        PrecioUnitario = g.Key.IdEdicionP.PrecioRendicion,
                        TotalCantidad = g.Sum( x => x.Cantidad),
                        TotalDevoluciones = g.Sum( x => x.Devoluciones),
                        TotalMonto = g.Sum( x=> x.Monto),
                        TotalImporte = (decimal) g.Sum( x=> x.Importe), 
                        TotalSaldo = g.Sum( x=> x.Saldo),
                        Distribuciones = g.ToList()

                    })
                .ToListAsync();



                // resumen general del reporte
                ResumenDistribuciones resumen = new ResumenDistribuciones()
                {
                    TotalDistribuciones = distribucionesAgrupadas.Sum(d => d.TotalMonto),
                    TotalIngresos = distribucionesAgrupadas.Sum(d => d.TotalImporte),
                    TotalDeudas = distribucionesAgrupadas.Sum(d => d.TotalSaldo),
                    FechaInicioResumen = fechaInicio,
                    FechaFinResumen = fechaFin
                };

                // enlazamos los querys a la vista
                var model = new Dictionary<string, object>
                {
                    ["DistribucionDetalleAgrupado"] = distribucionesAgrupadas,
                    ["Resumen"] = resumen,
                    ["DistribucionV"] = distribucionV,
                    ["DistribucionDetalleAgrupadoV"] = distribucionesAgrupadasVendedor,
                    // si hay mas querys agregamos aqui
                };

                // esta parte va a ser igual en todos los reportes
                // lo unico que cambiaria el nombre de la vista en el RenderAsync() 
                // y el nombre del reporte en el File()
                var wkhtmltopdfpath = _configuration.GetSection("Reportes:WkBinPath").Get<string>();
                var html = await _viewRender.RenderAsync("reporte_ventas_articulos", model);
                var wkhtmltopdf = new FileInfo(wkhtmltopdfpath);
                var converter = new HtmlToPdfConverter(wkhtmltopdf);
                var pdf = converter.ConvertToPdf(html);

                return File(pdf, MediaTypeNames.Application.Pdf,
                        $"Reporte Ventas Articulos {DateTime.Now:yyyyMMdd-hhmmss}.pdf");

            }
            return NoContent();
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
        public decimal? PrecioVenta { get; set; }
        public decimal? PrecioRendicion { get; set; }
    }


    public class ResumenDistribuciones
    {
        public Decimal TotalDistribuciones { get; set; }
        public Decimal TotalIngresos { get; set; }
        public Decimal TotalDeudas { get; set; }
        public DateTime FechaInicioResumen {get; set;}
        public DateTime FechaFinResumen {get; set;}
    }

    public class DistribucionDetalleAgrupado
    {   //distribuciones agrupadas por vendedor
        public long IdVendedor { get; set; }
        public Decimal TotalMonto {get; set;}
        public Decimal TotalImporte {get; set;}
        public Decimal TotalSaldo {get; set;}
        public IEnumerable<DistribucionDetalle> Distribuciones { get; set; }
        public DateTime FechaCreacion {get; set;}
        public long IdArticulo {get; set;}
        public long TotalCantidad {get; set;}
        public long? TotalDevoluciones {get; set;}
        public long IdEdicion {get; set;}
        public String NombreVendedor {get; set;}
        public decimal? PrecioUnitario {get; set;}
        public string NombreArticulo {get; set;}
        public long NroEdicion {get; set;}
        public DateTime? FechaEdicion {get; set;}
    }

    public class DistribucionFecha{
        public DateTime FechaCreacion {get; set;}
        public IEnumerable<Distribucion> DistribucionesFecha { get; set; }
    }

}