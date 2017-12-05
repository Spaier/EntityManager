using System;

namespace EntityManager.Identity
{
    /// <summary>
    /// Entity owned by identity user.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TUserKey"></typeparam>
    public interface IUserEntity<TUser, TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        /// <summary>
        /// Owner of the entity.
        /// </summary>
        TUser User { get; set; }

        /// <summary>
        /// Owner's id.
        /// </summary>
        TUserKey UserId { get; set; }
    }
}
