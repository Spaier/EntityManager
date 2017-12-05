using EntityManager.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityManager.AspNetCore.Identity
{
    /// <summary>
    /// Authorization handler that decides whether user can CRUD an entity or entities.
    /// </summary>
    /// <typeparam name="TUserEntity"></typeparam>
    /// <typeparam name="TUserManager"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TUserKey"></typeparam>
    public class UserIsOwnerAuthorizationHandler<TUserEntity, TUserManager, TUser, TUserKey>
        : AuthorizationHandler<OperationAuthorizationRequirement, TUserEntity>
        where TUserEntity : IUserEntity<TUser, TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>
        where TUserManager : UserManager<TUser>
    {
        private readonly TUserManager _userManager;

        /// <summary>
        /// Create with given user store.
        /// </summary>
        /// <param name="userManager"></param>
        public UserIsOwnerAuthorizationHandler(TUserManager userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Handle request.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <param name="resource"></param>
        /// <returns>Returns task for handling requirement.</returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
            TUserEntity resource)
        {
            if (context.User != null && resource != null &&
                EqualityComparer<TUserKey>.Default.Equals(resource.UserId, (await _userManager.GetUserAsync(context.User)).Id))
            {
                context.Succeed(requirement);
            }
        }
    }
}
