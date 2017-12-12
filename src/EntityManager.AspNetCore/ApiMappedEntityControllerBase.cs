﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace EntityManager.AspNetCore
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEntityViewModel"></typeparam>
    public abstract class ApiMappedEntityControllerBase<TDbContext, TEntity, TEntityViewModel> : SimpleApiEntityController<TDbContext, TEntity>
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
        public ApiMappedEntityControllerBase(TDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        [NonAction]
        public override Task<IActionResult> PostEntity([FromBody, Required] TEntity entity) => throw new NotSupportedException();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public override Task<IActionResult> PutEntity([FromBody, Required] TEntity entity) => throw new NotSupportedException();

        /// <summary>
        /// Convets a viewmodel to an entity and adds it to a database and returns 204.
        /// </summary>
        /// <param name="entityViewModel"></param>
        /// <returns></returns>
        /// <response code="204">Success.</response>
        [ProducesResponseType(204)]
        [HttpPost]
        public Task<IActionResult> PostEntity([FromBody, Required] TEntityViewModel entityViewModel) => PostEntityInner(entityViewModel);

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
        public Task<IActionResult> PutEntity([FromBody, Required] TEntityViewModel entityViewModel) => PutEntityInner(entityViewModel);

        /// <summary>
        /// Convets a viewmodel to an entity, adds it to a database and returns <see cref="NoContentResult"/>.
        /// </summary>
        /// <param name="entityViewModel"></param>
        protected Task<IActionResult> PostEntityInner(TEntityViewModel entityViewModel)
        {
            var entity = _mapper.Map<TEntityViewModel, TEntity>(entityViewModel);
            return PostEntityInner(entity);
        }

        /// <summary>
        /// Convets a viewmodel to an entity, updates it and returns <see cref="NoContentResult"/> or
        /// <see cref="NotFoundResult"/> if it doesn't exist.
        /// </summary>
        /// <param name="entityViewModel"></param>
        protected Task<IActionResult> PutEntityInner(TEntityViewModel entityViewModel)
        {
            var entity = _mapper.Map<TEntityViewModel, TEntity>(entityViewModel);
            return PutEntityInner(entity);
        }
    }
}
