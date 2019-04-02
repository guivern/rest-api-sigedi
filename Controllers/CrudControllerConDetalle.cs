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
    public abstract class CrudControllerConDetalle<TEntity, TDto, TEntityDetalle, TDtoDetalle> : CrudControllerBase<TEntity, TDto> where TEntity : EntityBaseConDetalle<TEntityDetalle>, new() where TDto : DtoConDetalle<TDtoDetalle> where TDtoDetalle : DtoBase, new() where TEntityDetalle : EntityBase
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
                // obtenemos los datos del dto y mapeamos a su clase entidad
                TEntity entity = _mapper.Map<TEntity>(dto);
                // realizamos alguna tarea antes de guardar
                await ExecuteBeforeSave(dto);
                // guardamos la cabecera
                await EntityDbSet.AddAsync(entity);
                await _context.SaveChangesAsync();

                // obtenemos los detalles del dto y mapeamos a su clase entidad
                IEnumerable<TEntityDetalle> detalles = _mapper.Map<IEnumerable<TEntityDetalle>>(dto.Detalle);

                // seteamos el Id padre de la relacion en cada detalle
                foreach (var detalle in detalles)
                {
                    foreach (var property in detalle.GetType().GetProperties())
                    {
                        var idPadre = property.GetCustomAttributes(typeof(IdPadre), false);
                        if (idPadre.Length > 0)
                        {
                            /* TODO: Agregar un break cuando se encuentre el Id padre 
                            o usar un while en lugar del for*/
                            property.SetValue(detalle, entity.Id);
                        }
                    }
                }
                // guardamos los detalles
                EntityDetalleDbSet.AddRange(detalles);
                await _context.SaveChangesAsync();
                // realizamos alguna tarea luego de guardar
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

                // obtenemos los datos del dto y mapeamos a su clase entidad 
                entity = _mapper.Map<TDto, TEntity>(dto, entity);
                if (IsAuditEntity)
                {
                    (entity as AuditEntityBase).FechaUltimaModificacion = DateTime.Now;
                }
                // realizamos alguna tarea antes de guardar
                await ExecuteBeforeSave(dto);
                // guardamos la cabecera
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // trabajamos con los detalles
                // obtenemos los detalles desde la base de datos
                var entityDb = await EntityDbSet
                .Include( e => e.Detalle)
                .Select( e => new {
                    e.Id,
                    detalle = e.Detalle
                })
                .SingleAsync(e => e.Id == entity.Id);

                // verificamos si hay detalles nuevos para agregar
                foreach (var detalleDto in dto.Detalle)
                {
                    if (detalleDto.Id == null)
                    {   // es nuevo
                        TEntityDetalle detalle = _mapper.Map<TEntityDetalle>(detalleDto);
                        // obtenemos y seteamos el id de la relacion padre
                        foreach (var property in detalle.GetType().GetProperties())
                        {
                            var idPadre = property.GetCustomAttributes(typeof(IdPadre), false);
                            if (idPadre.Length > 0)
                            {
                                property.SetValue(detalle, entity.Id);
                            }
                        }
                        // guardamos el nuevo detalle
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
                        detalle = _mapper.Map<TDtoDetalle, TEntityDetalle>(detalleDto, detalle);

                        _context.Entry(detalle).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                
                // eliminamos los detalles que ya no se encuentran en el dto
                foreach (var detDb in entityDb.detalle)
                {
                    var seElimina = true;
                    foreach (var detDto in dto.Detalle)
                    {
                        if (detDb.Id == detDto.Id)
                        {
                            seElimina = false;
                        }
                    }
                    if (seElimina)
                    {
                        EntityDetalleDbSet.Remove(detDb);
                        await _context.SaveChangesAsync();
                    }
                }
                // realizamos alguna tarea luego de guardar
                await ExecutePostSave(dto);
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        // sobreescribir si se desea realizar algo antes de guardar
        protected async virtual Task ExecuteBeforeSave(TDto dto)
        { }

        // sobreescribir si se desea realizar algo luego de guardar
        protected async virtual Task ExecutePostSave(TDto dto)
        { }
    }
}