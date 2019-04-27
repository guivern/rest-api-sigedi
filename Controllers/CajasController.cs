using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rest_api_sigedi.Annotations;
using rest_api_sigedi.Models;
using rest_api_sigedi.Utils;
using WkWrap.Core;

namespace rest_api_sigedi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CajasController : CrudControllerSinDetalle<Caja, CajaDto>
    {
        private readonly ViewRender _viewRender;

        public CajasController(DataContext context, IMapper mapper, ViewRender viewRender) : base(context, mapper)
        {
            _viewRender = viewRender;
        }

        protected override IQueryable<Caja> IncludeListFields(IQueryable<Caja> query)
        {
            return query
            .Include(c => c.UsuarioCreador);
        }

        [HttpGet("cajero/{idCajero}")]
        public async Task<IActionResult> GetCajaActiva(long idCajero)
        {
            var caja = await _context.Cajas
            .Include(c => c.UsuarioCreador)
            .Where(c => c.IdUsuarioCreador == idCajero && c.Activo)
            .SingleOrDefaultAsync();

            if (caja == null) return NotFound();

            return Ok(caja);
        }

        public override async Task<IActionResult> Create(CajaDto dto)
        {
            // solo debe existir una caja activa por usuario
            // vamos a desactivar las cajas previamente activas del usuario
            var cajas = await _context.Cajas
            .Where(c => c.IdUsuarioCreador == dto.IdUsuarioCreador)
            .ToListAsync();

            foreach (var caja in cajas)
            {
                caja.Activo = false;
            }

            _context.UpdateRange(cajas);

            dto.Monto = dto.MontoInicial;
            return await base.Create(dto);
        }

        [HttpGet("reporte/{idCaja}")]
        public async Task<IActionResult> GetReporte(long idCaja)
        {
            // realizamos los querys para el reporte
            var caja = await _context.Cajas
            .Include(c => c.UsuarioCreador)
            .SingleOrDefaultAsync(c => c.Id == idCaja);
            if (caja == null) return NotFound();

            var rendiciones = await _context.RendicionDetalles
            .Include(r => r.Rendicion)
            .ThenInclude(r => r.Vendedor)
            .Include(r => r.DistribucionDetalle)
            .ThenInclude(d => d.Edicion)
            .ThenInclude(e => e.Articulo)
            .Include(r => r.DistribucionDetalle)
            .ThenInclude(d => d.Edicion)
            .ThenInclude(e => e.Precio)
            .Where(r => r.Rendicion.IdCaja == idCaja)
            .OrderBy(r => r.Id)
            .ToListAsync();

            ResumenRendiciones resumen = new ResumenRendiciones()
            {
                TotalRendiciones = rendiciones.Sum(r => r.Monto),
                TotalIngresos = rendiciones.Sum(r => r.Importe),
                TotalDeudas = rendiciones.Sum(r => r.Saldo)
            };

            // enlazamos los querys a la vista
            var model = new Dictionary<string, object>
            {
                ["Caja"] = caja,
                ["Rendiciones"] = rendiciones,
                ["Resumen"] = resumen
                // si hay mas querys agregamos aqui
            };

            // esta parte va a ser igual en todos los reportes
            // lo unico que cambiaria el nombre de la vista en el RenderAsync() 
            // y el nombre del reporte en el File()
            var wkhtmltopdfpath = "/usr/local/bin/wkhtmltopdf";
            var html = await _viewRender.RenderAsync("reporte_caja", model);
            var wkhtmltopdf = new FileInfo(wkhtmltopdfpath);
            var converter = new HtmlToPdfConverter(wkhtmltopdf);
            var pdf = converter.ConvertToPdf(html);

            return File(pdf, MediaTypeNames.Application.Pdf,
                    $"Reporte Caja Id {caja.Id} - {DateTime.Now:yyyyMMdd-hhmmss}.pdf");
        }

        /*al desactivar una caja ya no se podran anular las rendiciones*/
        [HttpPut("[action]/{id}")]
        public override async Task<IActionResult> Desactivar(long id)
        {
            var caja = await _context.Cajas.SingleOrDefaultAsync(c => c.Id == id);
            if (caja == null) return NotFound();

            caja.Activo = false;
            caja.FechaCierre = DateTime.Now;
            _context.Cajas.Update(caja);
            await _context.SaveChangesAsync();

            var rendiciones = await _context.Rendiciones
            .Where(r => r.IdCaja == id)
            .ToListAsync();

            foreach (var rendicion in rendiciones)
            {
                rendicion.Anulable = false;
            }

            _context.Rendiciones.UpdateRange(rendiciones);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class CajaDto : DtoBase
    {
        [Requerido]
        public long? IdUsuarioCreador { get; set; }
        public DateTime? FechaCierre { get; set; }
        [Requerido]
        [NoNegativo]
        public Decimal? MontoInicial { get; set; } = 0;
        public Decimal? Monto { get; set; } = 0;
    }

    public class ResumenRendiciones
    {
        public Decimal TotalRendiciones { get; set; }
        public Decimal TotalIngresos { get; set; }
        public Decimal TotalDeudas { get; set; }
    }
}