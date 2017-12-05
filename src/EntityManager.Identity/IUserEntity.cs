using System;

namespace EntityManager.Identity
{
    /// <summary>
    /// Entity owned by a user.
    /// </summary>
    /// <typeparam name="TUser">The type encapsulation a user.</typeparam>
    /// <typeparam name="TUserKey">The type used for the primary key for the user.</typeparam>
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
