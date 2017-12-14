using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityManager.AspNetCore
{
    public class ApiUserEntityController<TDbContext, TUserManager, TRole, TUserEntity, TUser, TUserKey, TUserEntityViewModel>
        : ApiAuthEntityController<TDbContext, TUserEntity, TUserEntityViewModel>
        where TUserEntity : class, IUserEntity<TUser, TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>
        where TRole : IdentityRole<TUserKey>
        where TUserManager : UserManager<TUser>
        where TDbContext : IdentityDbContext<TUser, TRole, TUserKey>
    {
        private readonly TUserManager _userManager;

        public ApiUserEntityController(TDbContext context, TUserManager userManager, IMapper mapper, IAuthorizationService authorizationService)
            : base(context, mapper, authorizationService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        protected override async Task<List<TUserEntity>> GetAllEntities()
        {
            var user = await GetUser();
            return await _context.Set<TUserEntity>()
                .AsNoTracking()
                .Where(entity => EqualityComparer<TUserKey>.Default.Equals(entity.UserId, user.Id))
                .ToListAsync();
        }

        protected Task<TUser> GetUser() => _userManager.GetUserAsync(User);

        private protected override async Task<TUserEntity> MapEntity(TUserEntityViewModel entityViewModel)
        {
            var entity = await base.MapEntity(entityViewModel);
            entity.UserId = (await GetUser()).Id;
            return entity;
        }
    }
}
