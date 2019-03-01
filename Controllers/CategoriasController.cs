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
    public class CategoriasController : CrudControllerBase<Categoria, CategoriaDto>
    {
        public CategoriasController(DataContext context, IMapper mapper) 
        : base(context, mapper)
        {}

        // evaluamos que no se repita un registro de categorias
        protected override async Task<bool> IsValidModel(CategoriaDto dto)
        {
            if (await _context.Categorias
            .AnyAsync(v => v.Descripcion == dto.Descripcion && v.Id != dto.Id))
            {
                ModelState.AddModelError(
                nameof(dto.Descripcion),
                $"Ya existe una categoria con esa descripción \"{dto.Descripcion}\" en el sistema");

                return false;
            }
            return true;
        }
    }

    public class CategoriaDto: DtoBase
    {
        [Requerido]
        public string Descripcion{ get; set;} 
        public bool? Activo { get; set;} = true;
    }
}