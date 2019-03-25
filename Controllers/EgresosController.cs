using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rest_api_sigedi.Annotations;
using rest_api_sigedi.Models;

namespace rest_api_sigedi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EgresosController : CrudControllerBase<Egreso, EgresoDto>
    {   
        public EgresosController(DataContext context, IMapper mapper) 
        : base(context, mapper)
        {}
        
        protected override IQueryable<Egreso> IncludeListFields(IQueryable<Egreso> query)
        {
            return query
            .Include(i => i.Edicion)
                .ThenInclude(a => a.Articulo)
            .Include(i => i.Edicion)
                .ThenInclude(a => a.Precio)
            .Include(i => i.UsuarioCreador);
        }
    }

    public class EgresoDto : DtoBase
    {
        public long IdEdicion {get; set;}
        public long? IdUsuarioCreador { get; set; }
        public long Cantidad {get; set;}
        public DateTime? Fecha {get; set;}
    }
}