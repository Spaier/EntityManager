using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace EntityManager.AspNetCore
{
    /// <summary>
    /// A base class for an API Controller managing entity with api/[controller] route. It has no overposting protection.
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    [Route("api/[controller]")]
    public abstract class SimpleApiEntityController<TDbContext, TEntity> : ApiEntityControllerBase<TDbContext, TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        /// <summary>
        /// Create with given database context.
        /// </summary>
        /// <param name="context"></param>
        public SimpleApiEntityController(TDbContext context) : base(context) { }

        /// <summary>
        /// Returns 200 and all entities.
        /// </summary>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpGet]
        [ProducesResponseType(200)]
        public virtual async Task<IActionResult> GetEntities() => Ok(await GetAllEntities());

        /// <summary>
        /// Returns 200 and entity with given primary key if it exists otherwise 404.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="404"></response>
        [HttpGet("{keyValues}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public virtual async Task<IActionResult> GetEntity(string[] keyValues)
        {
            var key = _context.GetKeyValues<TEntity>(keyValues);
            var entity = await GetOneEntity(keyValues);
            return await GetEntityActionResult(entity);
        }

        /// <summary>
        /// Adds an entity and returns 204.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <response code="204">Success.</response>
        [HttpPost]
        [ProducesResponseType(204)]
        public virtual Task<IActionResult> PostEntity([FromBody, Required] TEntity entity) => PostEntityInner(entity);

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
        public virtual Task<IActionResult> PutEntity([FromBody, Required] TEntity entity) => PutEntityInner(entity);

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
        public virtual async Task<IActionResult> DeleteEntity(string[] keyValues)
        {
            var key = _context.GetKeyValues<TEntity>(keyValues);
            var entity = await _context.FindAsync<TEntity>(key);
            return await DeleteEntityInner(entity);
        }
    }
}
