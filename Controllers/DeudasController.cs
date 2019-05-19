using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using rest_api_sigedi.Models;
using rest_api_sigedi.Utils;
using WkWrap.Core;

namespace rest_api_sigedi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeudasController: ControllerBase
    {
        private readonly DataContext _context;
        private readonly ViewRender _viewRender;
        private readonly IConfiguration _configuration;

        public DeudasController(DataContext context, ViewRender viewRender, IConfiguration configuration)
        {
            _context = context;
            _viewRender = viewRender;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var deudas = await _context.DistribucionDetalles
            .Include(d => d.Distribucion)
            .ThenInclude(d => d.Vendedor)
            .Where(d => d.Activo && !d.Anulado)
            .GroupBy(d => d.Distribucion.Vendedor)
            .Select(g => new{
                IdVendedor = g.Key.Id,
                Vendedor = g.Key.NombreCompleto,
                MontoDeuda = g.Sum(d => d.Saldo)
            }).ToListAsync();

            return Ok(deudas);
        }

        [HttpGet("vendedor/{id}")]
        public async Task<IActionResult> GetDetalle(long id)
        {
            var deuda = await _context.DistribucionDetalles
            .Include(d => d.Distribucion)
            .ThenInclude(d => d.Vendedor)
            .Include(d => d.Edicion)
            .ThenInclude(e => e.Articulo)
            .Where(d => d.Distribucion.Vendedor.Id == id)
            .Where(d => d.Activo && !d.Anulado)
            .GroupBy(d => d.Distribucion.Vendedor)
            .Select(g => new{
                Vendedor = g.Key.NombreCompleto,
                NroDocumento = g.Key.NumeroDocumento,
                TotalDeuda = g.Sum(d => d.Saldo),
                Detalle = g.ToList()
            })
            .SingleOrDefaultAsync();
            // no es necesario
            if(deuda == null) return NoContent();

            return Ok(deuda);
        }

        [HttpGet("reporte")]
        public async Task<IActionResult> GetReporte(){
            
            IEnumerable<DistribucionDetalleAgrupado> distribucionesAgrupadas =
            await _context.DistribucionDetalles
            .Include(d => d.Distribucion)
            .ThenInclude(d => d.Vendedor)
            .Include(d => d.Edicion)
            .ThenInclude(e => e.Articulo)
            .Include(d => d.Edicion)
            .ThenInclude(e => e.Precio)
            .Where(d => d.Activo && !d.Anulado)
            .OrderBy(r => r.Distribucion.FechaCreacion)
            .GroupBy( //agrupamos distribuciones por articulo
                c => new {
                IdVendedorP = c.Distribucion.Vendedor.Id,        
            })
            .Select(g => new DistribucionDetalleAgrupado(){
                IdVendedor = g.Key.IdVendedorP,
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
            .Where(d => d.Activo && !d.Anulado)
            .OrderBy(r => r.Distribucion.FechaCreacion)
            .GroupBy( //agrupamos distribuciones por articulo
                c => new {
                    IdArticuloP = c.Edicion.Articulo,
                    IdEdicionP = c.Edicion,
                    IdVendedorP = c.Distribucion.Vendedor
                })
                .Select(g => new DistribucionDetalleAgrupado(){
                    IdArticulo = g.Key.IdArticuloP.Id,
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
                TotalDeudas = distribucionesAgrupadas.Sum(d => d.TotalSaldo)
            };

            // enlazamos los querys a la vista
            var model = new Dictionary<string, object>
            {
                ["DistribucionDetalleAgrupado"] = distribucionesAgrupadas,
                ["Resumen"] = resumen,
                ["DistribucionDetalleAgrupadoAV"] = distribucionesAgrupadasAV,
                // si hay mas querys agregamos aqui
            };

            // esta parte va a ser igual en todos los reportes
            // lo unico que cambiaria el nombre de la vista en el RenderAsync() 
            // y el nombre del reporte en el File()
            var wkhtmltopdfpath = _configuration.GetSection("Reportes:WkBinPath").Get<string>();
            var html = await _viewRender.RenderAsync("reporte_deudas", model);
            var wkhtmltopdf = new FileInfo(wkhtmltopdfpath);
            var converter = new HtmlToPdfConverter(wkhtmltopdf);
            var pdf = converter.ConvertToPdf(html);

            return File(pdf, MediaTypeNames.Application.Pdf,
                $"Reporte Deudas {DateTime.Now:yyyyMMdd-hhmmss}.pdf");

        }

    }
}