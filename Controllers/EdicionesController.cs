using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

    }


    public class EdicionDto : DtoBase
    {
        
        public long IdArticulo {get; set;}
        public long IdPrecio {get; set;}
        public DateTime? FechaEdicion {get; set;}
        public long NroEdicion {get; set;}
        public long CantidadInicial {get; set;}
        public long CantidadActual {get; set;}
    }
}