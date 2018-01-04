using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityManager.AspNetCore
{
    /// <summary>
    /// A base class for an API Controller managing entities with EF Core.
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ApiEntityControllerBase<TDbContext, TEntity> : Controller
        where TDbContext : DbContext
        where TEntity : class
    {
        /// <summary>
        /// EF Core Database context.
        /// </summary>
        protected readonly TDbContext _context;

        /// <summary>
        /// Create with given database context.
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiEntityControllerBase(TDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Returns all entities. Override this to add includes/orderby/etc.
        /// </summary>
        /// <returns></returns>
        protected virtual Task<List<TEntity>> GetAllEntities() => _context.Set<TEntity>().AsNoTracking().ToListAsync();

        /// <summary>
        /// Returns entity with given primary key if it exists otherwise null. Override this to add includes/orderby/etc.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        protected virtual async Task<TEntity> GetOneEntity(object[] keyValues)
            => await _context.GetByPrimaryKeyAsync<TEntity>(keyValues);

        /// <summary>
        /// Updates an entity. Override this to update join entities.
        /// </summary>
        /// <param name="entity"></param>
        protected virtual Task UpdateEntity(TEntity entity)
        {
            _context.Update(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns <see cref="OkObjectResult"/> if entity with given primary key exists otherwise <see cref="NotFoundResult"></see>.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private protected virtual Task<IActionResult> GetEntityActionResult(TEntity entity)
        {
            if (entity == null) { return Task.FromResult((IActionResult)NotFound()); }
            return Task.FromResult((IActionResult)Ok(entity));
        }

        /// <summary>
        /// Adds an entity to a database and returns <see cref="NoContentResult"/>.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private protected virtual async Task<IActionResult> PostEntityInner(TEntity entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Updates an entity and returns <see cref="NoContentResult"/>. If it doesn't exist - <see cref="NotFoundResult"/>.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private protected virtual async Task<IActionResult> PutEntityInner(TEntity entity)
        {
            if (await _context.AnyByEntityAsync(entity))
            {
                await UpdateEntity(entity);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return NotFound();
        }

        /// <summary>
        /// Deletes an entity by primary key and returns <see cref="NoContentResult"/> or <see cref="NotFoundResult"/> if no entity with
        /// given primary key exists.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        /// <returns></returns>
        private protected virtual async Task<IActionResult> DeleteEntityInner(TEntity entity)
        {
            if (entity == null) { return NotFound(); }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
