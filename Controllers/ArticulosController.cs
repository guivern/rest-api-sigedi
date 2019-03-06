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
    public class ArticulosController : CrudControllerBase<Articulo, ArticuloDto>
    {
        public ArticulosController(DataContext context, IMapper mapper) : base(context, mapper)
        { }

        public override async Task<IActionResult> Detail(long id, [FromQuery] bool Inactivo)
        {
            var query = _context.Articulos.AsQueryable();
            if (!Inactivo)
            {
                query = query.Where(a => a.Activo);
            }

            var articulo = await IncludeListFields(query)
            .Include(a => a.Precios)
            .Select(a => new
            {
                a.Id,
                a.Codigo,
                a.Descripcion,
                a.IdCategoria,
                a.IdProveedor,
                a.Activo,
                Precios = a.Precios.Where(p => p.Activo).OrderByDescending(p => p.Id)
            })
            .SingleOrDefaultAsync(a => a.Id == id);

            if (articulo == null)
                return NotFound();

            return Ok(articulo);
        }

        public override async Task<IActionResult> Create(ArticuloDto dto)
        {
            if (await IsValidModel(dto))
            {
                Articulo articulo = _mapper.Map<Articulo>(dto);
                await EntityDbSet.AddAsync(articulo);
                await _context.SaveChangesAsync();

                foreach (var precioDto in dto.Precios)
                {
                    Precio precio = new Precio
                    {
                        IdArticulo = articulo.Id,
                        Descripcion = precioDto.Descripcion,
                        PrecioVenta = (decimal)precioDto.PrecioVenta,
                        PrecioRendVendedor = (decimal)precioDto.PrecioRendVendedor,
                        PrecioRendAgencia = precioDto.PrecioRendAgencia,
                    };
                    _context.Precios.Add(precio);
                }

                await _context.SaveChangesAsync();
                return CreatedAtAction("Detail", new { id = articulo.Id }, articulo);
            }
            return BadRequest(ModelState);

        }

        public override async Task<IActionResult> Update(long id, ArticuloDto dto)
        {
            if (id != dto.Id || dto.Id == null) return BadRequest();
            if (await IsValidModel(dto))
            {
                var articulo = await _context.Articulos.FindAsync(id);
                if (articulo == null) return NotFound();

                articulo = _mapper.Map<ArticuloDto, Articulo>(dto, articulo);
                _context.Entry(articulo).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                //obtenemos los precios del articulo
                var precios = await _context.Precios.Where(p => p.IdArticulo == id).ToListAsync();
                
                //verificamos si hay nuevos precios
                foreach (var PrecioDto in dto.Precios)
                {
                    if (PrecioDto.Id == null)
                    {
                        _context.Precios.Add(new Precio
                        {
                            Descripcion = PrecioDto.Descripcion,
                            PrecioVenta = (long)PrecioDto.PrecioVenta,
                            PrecioRendVendedor = (long)PrecioDto.PrecioRendVendedor,
                            PrecioRendAgencia = PrecioDto.PrecioRendAgencia,
                            IdArticulo = (long)dto.Id
                        });
                        await _context.SaveChangesAsync();
                    }
                }

                //verificamos si hay precios para eliminar
                foreach (var precio in precios)
                {
                    var seElimina = true;
                    foreach (var precioDto in dto.Precios)
                    {
                        if (precio.Id == precioDto.Id)
                        {
                            seElimina = false;
                        }
                    }
                    if (seElimina)
                    {
                        precio.Activo = false;
                        _context.Precios.Update(precio);
                        await _context.SaveChangesAsync();
                    }
                }

                return NoContent();
            }
            return BadRequest(ModelState);
        }

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
            if (dto.Precios.Count == 0)
            {
                ModelState.AddModelError(
                nameof(dto.Precios), "Debe ingresar al menos un precio de venta y rendición");
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
    }

    public class ArticuloDto : DtoBase
    {
        [Requerido]
        public string Descripcion { get; set; }
        [Requerido]
        public long? IdCategoria { get; set; }
        [Requerido]
        public long? IdProveedor { get; set; }

        public List<PrecioDto> Precios { get; set; } = new List<PrecioDto>();
    }

    public class PrecioDto : DtoBase
    {
        public string Descripcion { get; set; }
        [Requerido]
        public decimal? PrecioVenta { get; set; }
        [Requerido]
        public decimal? PrecioRendVendedor { get; set; }
        public decimal? PrecioRendAgencia { get; set; }
    }
}