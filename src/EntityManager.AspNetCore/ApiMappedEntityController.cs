using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace EntityManager.AspNetCore
{
    /// <summary>
    /// A base class for an API Controller managing entity with api/[controller] route and overposting protection.
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEntityViewModel"></typeparam>
    public abstract class ApiMappedEntityController<TDbContext, TEntity, TEntityViewModel> : SimpleApiEntityController<TDbContext, TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// Create with given database context and mapper instance.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiMappedEntityController(TDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        [NonAction]
        public sealed override Task<IActionResult> PostEntity([FromBody, Required] TEntity entity) => throw new NotSupportedException();

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public sealed override Task<IActionResult> PutEntity([FromBody, Required] TEntity entity) => throw new NotSupportedException();

        /// <summary>
        /// Convets a viewmodel to an entity and adds it to a database and returns 204.
        /// </summary>
        /// <param name="entityViewModel"></param>
        /// <returns></returns>
        /// <response code="204">Success.</response>
        [ProducesResponseType(204)]
        [HttpPost]
        public virtual async Task<IActionResult> PostEntity([FromBody, Required] TEntityViewModel entityViewModel)
        {
            var entity = await MapEntity(entityViewModel);
            return await PostEntityInner(entity);
        }

        /// <summary>
        /// Convets a viewmodel to an entity, updates it and returns 204 or
        /// 404 if it doesn't exist.
        /// </summary>
        /// <param name="entityViewModel"></param>
        /// <returns></returns>
        /// <response code="204">Success.</response>
        /// <response code="404">Not Found.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpPut]
        public virtual async Task<IActionResult> PutEntity([FromBody, Required] TEntityViewModel entityViewModel)
        {
            var entity = await MapEntity(entityViewModel);
            return await PutEntityInner(entity);
        }

        /// <summary>
        /// Convets a viewmodel to an entity.
        /// </summary>
        /// <param name="entityViewModel">ViewModel to convert.</param>
        /// <returns>Entity.</returns>
        private protected virtual Task<TEntity> MapEntity(TEntityViewModel entityViewModel)
            => Task.FromResult(_mapper.Map<TEntityViewModel, TEntity>(entityViewModel));
    }
}
