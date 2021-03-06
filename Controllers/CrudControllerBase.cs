using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rest_api_sigedi.Models;

namespace rest_api_sigedi.Controllers
{
    public abstract class CrudControllerBase<TEntity, TDto> : ControllerBase where TEntity : EntityBase, new() where TDto : DtoBase
    {
        protected readonly DataContext _context;
        protected readonly IMapper _mapper;
        protected readonly DbSet<TEntity> EntityDbSet;
        protected readonly Type EntityType;
        protected readonly bool IsSoftDelete;
        protected readonly bool IsAuditEntity;

        public CrudControllerBase(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            EntityDbSet = _context.Set<TEntity>();
            EntityType = typeof(TEntity);
            IsSoftDelete = EntityType.IsSubclassOf(typeof(SoftDeleteEntityBase)) ? true : false;
            IsAuditEntity = EntityType.IsSubclassOf(typeof(AuditEntityBase)) ? true : false;
        }

        [HttpGet]
        public virtual async Task<IActionResult> List([FromQuery] bool Inactivos)
        {
            var query = (IQueryable<TEntity>)EntityDbSet;

            if (IsSoftDelete && !Inactivos)
            {
                query = query.Where(e => (e as SoftDeleteEntityBase).Activo);
            }

            var result = await IncludeListFields(query)
            .OrderByDescending(e => e.Id)
            .ToListAsync(); 

            return Ok(result);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Detail(long id, [FromQuery] bool Inactivo)
        {
            var query = (IQueryable<TEntity>)EntityDbSet;
            if (IsSoftDelete && !Inactivo)
            {
                query = query.Where(e => (e as SoftDeleteEntityBase).Activo);
            }

            var entidad = await IncludeDetailFields(IncludeListFields(query)).SingleOrDefaultAsync(e => (e as EntityBase).Id == id);

            if (entidad == null)
                return NotFound();

            return Ok(entidad);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var entity = await EntityDbSet.FindAsync(id);

            if (entity == null) return NotFound();

            EntityDbSet.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> Activar(long id)
        {
            if (IsSoftDelete)
            {
                var entity = await EntityDbSet.FindAsync(id);

                if (entity == null) return NotFound();

                (entity as SoftDeleteEntityBase).Activo = true;

                if (IsAuditEntity)
                {
                    (entity as AuditEntityBase).FechaUltimaModificacion = DateTime.Now;
                }

                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return StatusCode(405);
        }

        [HttpPut("[action]/{id}")]
        public virtual async Task<IActionResult> Desactivar(long id)
        {
            if (IsSoftDelete)
            {
                var entity = await EntityDbSet.FindAsync(id);

                if (entity == null) return NotFound();

                (entity as SoftDeleteEntityBase).Activo = false;

                if (IsAuditEntity)
                {
                    (entity as AuditEntityBase).FechaUltimaModificacion = DateTime.Now;
                }

                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return StatusCode(405);
        }

        // sobreescribir este metodo si se desea incluir datos relacionales
        protected virtual IQueryable<TEntity> IncludeListFields(IQueryable<TEntity> query)
        {
            return query;
        }

        protected virtual IQueryable<TEntity> IncludeDetailFields(IQueryable<TEntity> query)
        {
            return query;
        }

        // sobreescribir este metodo si se desea realizar validaciones
        protected virtual async Task<bool> IsValidModel(TDto dto)
        {
            return ModelState.IsValid;
        }
    }
}