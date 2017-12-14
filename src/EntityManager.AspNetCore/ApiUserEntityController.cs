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
    public class ApiUserEntityController<TDbContext, TUserEntity, TUserEntityViewModel, TUserManager, TUser, TUserKey, TRole>
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

    public class ApiUserEntityController<TDbContext, TUserEntity, TUserEntityViewModel, TUserManager, TUser, TUserKey>
        : ApiUserEntityController<TDbContext, TUserEntity, TUserEntityViewModel, TUserManager, TUser, TUserKey, IdentityRole<TUserKey>>
        where TUserEntity : class, IUserEntity<TUser, TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>
        where TUserManager : UserManager<TUser>
        where TDbContext : IdentityDbContext<TUser, IdentityRole<TUserKey>, TUserKey>
    {
        public ApiUserEntityController(TDbContext context, TUserManager userManager, IMapper mapper, IAuthorizationService authorizationService)
            : base(context, userManager, mapper, authorizationService) { }
    }

    public class ApiUserEntityController<TDbContext, TUserEntity, TUserEntityViewModel, TUser, TUserKey>
        : ApiUserEntityController<TDbContext, TUserEntity, TUserEntityViewModel, UserManager<TUser>, TUser, TUserKey, IdentityRole<TUserKey>>
        where TUserEntity : class, IUserEntity<TUser, TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>
        where TDbContext : IdentityDbContext<TUser, IdentityRole<TUserKey>, TUserKey>
    {
        public ApiUserEntityController(TDbContext context, UserManager<TUser> userManager, IMapper mapper, IAuthorizationService authorizationService)
            : base(context, userManager, mapper, authorizationService) { }
    }
}
