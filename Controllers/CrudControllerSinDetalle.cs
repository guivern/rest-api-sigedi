using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rest_api_sigedi.Models;

namespace rest_api_sigedi.Controllers
{
    public abstract class CrudControllerSinDetalle<TEntity, TDto> : CrudControllerBase<TEntity, TDto> where TEntity : EntityBase, new() where TDto : DtoBase
    {
        public CrudControllerSinDetalle(DataContext context, IMapper mapper)
        : base(context, mapper)
        { }
        
        [HttpPost]
        public virtual async Task<IActionResult> Create(TDto dto)
        {
            if (await IsValidModel(dto))
            {
                TEntity entity = _mapper.Map<TEntity>(dto);
                await EntityDbSet.AddAsync(entity);
                await _context.SaveChangesAsync();

                return CreatedAtAction("Detail", new { id = entity.Id }, entity);
            }

            return BadRequest(ModelState);

        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(long id, TDto dto)
        {
            if (id != dto.Id || dto.Id == null) return BadRequest();

            if (await IsValidModel(dto))
            {
                var entity = await EntityDbSet.FindAsync(id);
                if (entity == null) return NotFound();

                entity = _mapper.Map<TDto, TEntity>(dto, entity);

                if (IsAuditEntity)
                {
                    (entity as AuditEntityBase).FechaUltimaModificacion = DateTime.Now;
                }

                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }

            return BadRequest(ModelState);
        }
    }
}