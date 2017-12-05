using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace EntityManager.AspNetCore
{
    /// <summary>
    /// A base class for an API Controller managing entity with api/[controller] route.
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    [Route("api/[controller]")]
    public abstract class ApiEntityController<TDbContext, TEntity> : Controller
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
        public ApiEntityController(TDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Returns all entities.
        /// </summary>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpGet]
        [ProducesResponseType(200)]
        public virtual Task<List<TEntity>> GetEntities() => _context.Set<TEntity>().AsNoTracking().ToListAsync();

        /// <summary>
        /// Returns 200 if entity with given primary key exists otherwise 404.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="404"></response>
        [HttpGet("{keyValues}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public virtual Task<IActionResult> GetEntity(string[] keyValues)
        {
            var key = _context.GetKeyValues<TEntity>(keyValues);
            return GetEntity(key);
        }

        /// <summary>
        /// Adds an entity and returns 204.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <response code="204">Success.</response>
        [HttpPost]
        [ProducesResponseType(204)]
        public virtual async Task<IActionResult> PostEntity([FromBody, Required] TEntity entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// If entity with given primary key exists updates it and returns 204 otherwise 404.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <response code="204">Success.</response>
        /// <response code="404">No entity found.</response>
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public virtual async Task<IActionResult> PutEntity([FromBody, Required] TEntity entity)
        {
            if (await _context.AnyByEntityAsync(entity))
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return NotFound();
        }

        /// <summary>
        /// Deletes an entity by primary key and returns 204 or 404 if no entity with
        /// given primary key exists.
        /// </summary>
        /// <param name="keyValues">Primary key of the entity.</param>
        /// <returns></returns>
        /// <response code="204"></response>
        /// <response code="404"></response>
        [HttpDelete("{keyValues}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public Task<IActionResult> DeleteEntity(string[] keyValues)
        {
            var key = _context.GetKeyValues<TEntity>(keyValues);
            return DeleteEntity(key);
        }

        /// <summary>
        /// Deletes an entity by primary key and returns <see cref="NoContentResult"/> or <see cref="NotFoundResult"/> if no entity with
        /// given primary key exists.
        /// </summary>
        /// <param name="keyValues">Primary key of the entity.</param>
        /// <returns></returns>
        protected async Task<IActionResult> DeleteEntity(object[] keyValues)
        {
            var entity = await _context.FindAsync<TEntity>(keyValues);
            if (entity == null) { return NotFound(); }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Returns <see cref="OkObjectResult"/> if entity with given primary key exists otherwise <see cref="NotFoundResult"></see>.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        protected async Task<IActionResult> GetEntity(object[] keyValues)
            => GetEntityJson(await _context.GetByPrimaryKeyAsync<TEntity>(keyValues));

        /// <summary>
        /// Returns <see cref="OkObjectResult"/> if <paramref name="entity"/> is not null otherwise <see cref="NotFoundResult"></see>.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected IActionResult GetEntityJson(TEntity entity)
        {
            if (entity == null) { return NotFound(); }
            return Ok(entity);
        }
    }
}
