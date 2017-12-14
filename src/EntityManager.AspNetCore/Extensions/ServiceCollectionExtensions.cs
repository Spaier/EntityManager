using EntityManager;
using EntityManager.AspNetCore;
using EntityManager.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Static class containing extension methods for registering services in asp.net core di.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services to di.
        /// </summary>
        /// <typeparam name="TUserEntity"></typeparam>
        /// <typeparam name="TUserManager"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TUserKey"></typeparam>
        /// <param name="serviceCollection"></param>
        public static void AddUserIsOwnerHandler<TUserEntity, TUserManager, TUser, TUserKey>(this IServiceCollection serviceCollection)
            where TUserEntity : IUserEntity<TUser, TUserKey>
            where TUserManager : UserManager<TUser>
            where TUser : IdentityUser<TUserKey>
            where TUserKey : IEquatable<TUserKey>
        {
            serviceCollection.AddScoped<IAuthorizationHandler, UserIsOwnerAuthorizationHandler<TUserEntity, TUserManager, TUser, TUserKey>>();
        }

        /// <summary>
        /// Adds admin auth handler for specified type.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="serviceCollection"></param>
        public static void AddAdminAuthHandler<TEntity>(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAuthorizationHandler>(w => new AdminAuthorizationHandler<TEntity>(""));
        }

        /// <summary>
        /// Adds admin auth handler for any type.
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void AddAdminAuthHandler(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped(typeof(IAuthorizationHandler), typeof(AdminAuthorizationHandler<>));
        }
    }
}
