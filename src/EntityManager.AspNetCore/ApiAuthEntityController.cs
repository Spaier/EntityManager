using AutoMapper;
using EntityManager.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EntityManager.AspNetCore
{
    public class ApiAuthEntityController<TDbContext, TEntity, TEntityViewModel> : ApiMappedEntityController<TDbContext, TEntity, TEntityViewModel>
        where TDbContext : DbContext
        where TEntity : class
    {
        protected readonly IAuthorizationService _authorizationService;

        public ApiAuthEntityController(TDbContext context, IMapper mapper, IAuthorizationService authorizationService) : base(context, mapper)
        {
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        }

        private protected override async Task<IActionResult> PostEntityInner(TEntity entity)
            => await HandleAuth(entity, CrudOperations.Create) ?? await base.PostEntityInner(entity);

        private protected override async Task<IActionResult> PutEntityInner(TEntity entity)
            => await HandleAuth(entity, CrudOperations.Update) ?? await base.PutEntityInner(entity);

        private protected override async Task<IActionResult> GetEntityActionResult(TEntity entity) =>
            await HandleAuth(entity, CrudOperations.Read) ?? await base.GetEntityActionResult(entity);

        private protected override async Task<IActionResult> DeleteEntityInner(TEntity entity) =>
            await HandleAuth(entity, CrudOperations.Delete) ?? await base.DeleteEntityInner(entity);

        private protected async Task<IActionResult> HandleAuth(TEntity entity, IAuthorizationRequirement requirement)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, entity, requirement);
            if (authResult.Succeeded)
            {
                return null;
            }
            if (User?.Identity?.IsAuthenticated == true) { return Forbid(); }
            return Challenge();
        }
    }
}
