using System;
using System.Collections.Generic;
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
    public class ArticulosController : CrudControllerConDetalle<Articulo, ArticuloDto, Precio, PrecioDto>
    {
        public ArticulosController(DataContext context, IMapper mapper) : base(context, mapper)
        { }

        protected override async Task<bool> IsValidModel(ArticuloDto dto)
        {
            if (await _context.Articulos
            .AnyAsync(a => a.Descripcion.ToLower().Equals(dto.Descripcion.ToLower()) && a.Id != dto.Id))
            {
                ModelState.AddModelError(
                nameof(dto.Descripcion),
                $"Ya existe un artículo con nombre \"{dto.Descripcion}\" en el sistema");
                return false;
            }
            if (await _context.Articulos
            .AnyAsync(a => a.Codigo == dto.Codigo && a.Id != dto.Id && a.Codigo != null))
            {
                ModelState.AddModelError(
                nameof(dto.Codigo),
                $"Ya existe un artículo con código \"{dto.Codigo}\" en el sistema");
                return false;
            }
            if (dto.Detalle.Count == 0 || !dto.Detalle.Any(d => (bool) d.Activo))
            {
                ModelState.AddModelError(
                nameof(dto.Detalle), "Debe ingresar al menos un precio de venta y rendición");
                return false;
            }
            return true;
        }

        protected override IQueryable<Articulo> IncludeListFields(IQueryable<Articulo> query)
        {
            return query
            .Include(a => a.Proveedor)
            .Include(a => a.Categoria);
        }

        protected override IQueryable<Articulo> IncludeDetailFields(IQueryable<Articulo> query)
        {
            return query.Include(a => a.Detalle).OrderByDescending(a => a.Id);
        }

    }

    public class ArticuloDto : DtoConDetalle<PrecioDto>
    {
        [Requerido]
        public string Descripcion { get; set; }
        public long? Codigo { get; set; }
        [Requerido]
        public long? IdCategoria { get; set; }
        [Requerido]
        public long? IdProveedor { get; set; }
    }

    public class PrecioDto : DtoBase
    {
        [Requerido]
        public string Descripcion { get; set; }
        [Requerido]
        [NoNegativo]
        public decimal? PrecioVenta { get; set; }
        [Requerido]
        [NoNegativo]
        public decimal? PrecioRendVendedor { get; set; }
        public decimal? PrecioRendAgencia { get; set; }
        public bool? Activo {get; set;} = true;
    }
}