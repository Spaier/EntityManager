using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Threading.Tasks;

namespace EntityManager.AspNetCore.Authorization
{
    /// <summary>
    /// Allows specified role to ignore auth handlers.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class AdminAuthorizationHandler<TEntity> : AuthorizationHandler<OperationAuthorizationRequirement, TEntity>
    {
        private readonly string _role;

        /// <summary>
        /// Create handler for given role name.
        /// </summary>
        /// <param name="role"></param>
        public AdminAuthorizationHandler(string role)
        {
            _role = role;
        }

        /// <summary>
        /// Handle request.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <param name="resource"></param>
        /// <returns>Task for handling request.</returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
            TEntity resource)
        {
            if (context.User != null && context.User.IsInRole(_role))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
