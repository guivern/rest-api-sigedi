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
    }

    public class IngresoDto : DtoConDetalle<IngresoDetalleDto>
    {
        [Requerido]
        public long? IdProveedor { get; set; }
        [Requerido]
        public long? IdUsuario { get; set; }
        public string TipoComprobante { get; set; }
        public string NroComprobante { get; set; }
    }

    public class IngresoDetalleDto : DtoBase
    {
        [Requerido]
        [NoNegativo]
        public long Cantidad { get; set; }
        [Requerido]
        public DateTime FechaEdicion { get; set; }
        [Requerido]
        public String NroEdicion { get; set; }
    }

}