using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
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
        /// Returns <see cref="OkObjectResult"/> if entity with given primary key exists otherwise <see cref="NotFoundResult"></see>.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        private protected async Task<IActionResult> GetEntityInner(object[] keyValues)
        {
            var entity = await GetOneEntity(keyValues);
            if (entity == null) { return NotFound(); }
            return Ok(entity);
        }

        /// <summary>
        /// Returns <see cref="OkObjectResult"/> if entity with given primary key exists otherwise <see cref="NotFoundResult"></see>.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        private protected Task<IActionResult> GetEntityByKey(string[] keyValues)
        {
            var key = _context.GetKeyValues<TEntity>(keyValues);
            return GetEntityInner(key);
        }

        /// <summary>
        /// Adds an entity to a database and returns <see cref="NoContentResult"/>.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private protected async Task<IActionResult> PostEntityInner(TEntity entity)
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
        private protected async Task<IActionResult> PutEntityInner(TEntity entity)
        {
            if (await _context.AnyByEntityAsync(entity))
            {
                UpdateEntity(entity);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return NotFound();
        }

        /// <summary>
        /// Deletes an entity by primary key and returns <see cref="NoContentResult"/> or <see cref="NotFoundResult"/> if no entity with
        /// given primary key exists.
        /// </summary>
        /// <param name="keyValues">Primary key of the entity.</param>
        /// <returns></returns>
        private protected async Task<IActionResult> DeleteEntityInner(object[] keyValues)
        {
            var entity = await _context.FindAsync<TEntity>(keyValues);
            if (entity == null) { return NotFound(); }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes an entity by primary key and returns <see cref="NoContentResult"/> or <see cref="NotFoundResult"/> if no entity with
        /// given primary key exists.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        private protected Task<IActionResult> DeleteEntityByKey(string[] keyValues)
        {
            var key = _context.GetKeyValues<TEntity>(keyValues);
            return DeleteEntityInner(key);
        }

        /// <summary>
        /// Returns all entities.
        /// </summary>
        /// <returns></returns>
        protected virtual Task<List<TEntity>> GetAllEntities() => _context.Set<TEntity>().AsNoTracking().ToListAsync();

        /// <summary>
        /// Returns entity with given primary key if it exists otherwise null.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        protected virtual async Task<TEntity> GetOneEntity(object[] keyValues)
            => await _context.GetByPrimaryKeyAsync<TEntity>(keyValues);

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void UpdateEntity(TEntity entity)
        {
            _context.Update(entity);
        }
    }
}
