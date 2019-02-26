using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using rest_api_sigedi.Annotations;
using rest_api_sigedi.Models;

namespace rest_api_sigedi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : CrudControllerBase<Categoria, CategoriaDto>
    {
        public CategoriasController(DataContext context, IMapper mapper) 
        : base(context, mapper)
        {}
    }

    public class CategoriaDto: DtoBase
    {
        [Requerido]
        public string Descripcion{ get; set;} 
        public bool? Activo { get; set;} = true;
    }
}