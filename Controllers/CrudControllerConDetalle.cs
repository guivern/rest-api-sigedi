using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rest_api_sigedi.Annotations;
using rest_api_sigedi.Models;

namespace rest_api_sigedi.Controllers
{
    public abstract class CrudControllerConDetalle<TEntity, TDto, TEntityDetalle, TDtoDetalle> : CrudControllerBase<TEntity, TDto> where TEntity : EntityBase, new() where TDto : DtoConDetalle<TDtoDetalle> where TDtoDetalle : DtoBase, new() where TEntityDetalle : EntityBase
    {
        protected readonly DbSet<TEntityDetalle> EntityDetalleDbSet;

        public CrudControllerConDetalle(DataContext context, IMapper mapper)
        : base(context, mapper)
        {
            EntityDetalleDbSet = _context.Set<TEntityDetalle>();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(TDto dto)
        {
            if (await IsValidModel(dto))
            {
                // obtenemos la entidad padre luego de mapear y guardamos
                TEntity entity = _mapper.Map<TEntity>(dto);
                await EntityDbSet.AddAsync(entity);
                await _context.SaveChangesAsync();

                // obtenemos los detalles luego de mapear y guardamos
                IEnumerable<TEntityDetalle> detalles = _mapper.Map<IEnumerable<TEntityDetalle>>(dto.Detalle);

                // seteamos el Id padre de la relacion en cada detalle
                foreach (var detalle in detalles)
                {
                    foreach (var property in detalle.GetType().GetProperties())
                    {
                        var idPadre = property.GetCustomAttributes(typeof(IdPadre), false);
                        if (idPadre.Length > 0)
                        {
                            property.SetValue(detalle, entity.Id);
                        }
                    }
                }

                EntityDetalleDbSet.AddRange(detalles);
                await _context.SaveChangesAsync();
                await ExecutePostSave(dto);

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

                // obtenemos los datos actualizados de la cabecera y guardamos 
                entity = _mapper.Map<TDto, TEntity>(dto, entity);
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // agregamos nuevos detalles
                foreach (var detalleDto in dto.Detalle)
                {
                    if (detalleDto.Id == null)
                    {   // es nuevo
                        TEntityDetalle detalle = _mapper.Map<TEntityDetalle>(detalleDto);

                        foreach (var property in detalle.GetType().GetProperties())
                        {
                            var idPadre = property.GetCustomAttributes(typeof(IdPadre), false);
                            if (idPadre.Length > 0)
                            {
                                property.SetValue(detalle, entity.Id);
                            }
                        }

                        EntityDetalleDbSet.Add(detalle);
                        await _context.SaveChangesAsync();
                    }
                }

                // actualizamos los detalles antiguos
                foreach (var detalleDto in dto.Detalle)
                {
                    if (detalleDto.Id != null)
                    {
                        TEntityDetalle detalle = await EntityDetalleDbSet.FindAsync(detalleDto.Id);
                        detalle = _mapper.Map<TDtoDetalle,TEntityDetalle>(detalleDto, detalle);

                        if (IsAuditEntity)
                        {
                            (detalle as AuditEntityBase).FechaUltimaModificacion = DateTime.Now;
                        }

                        _context.Entry(detalle).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                await ExecutePostSave(dto);
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        // sobreescribir si se desea realizar algo con los detalles luego de guardar
        protected async virtual Task ExecutePostSave(TDto dto)
        {}
    }
}