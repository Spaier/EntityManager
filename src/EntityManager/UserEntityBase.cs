using System;

namespace EntityManager
{
    /// <summary>
    /// Base user's item with given id, user's id and user types.
    /// </summary>
    /// <typeparam name="TKey">The type used for the primary key for the entity.</typeparam>
    /// <typeparam name="TUser">The type encapsulating a user.</typeparam>
    /// <typeparam name="TUserKey">The type used for the primary key for the user.</typeparam>
    public abstract class UserEntityBase<TKey, TUser, TUserKey> : EntityBase<TKey>, IUserEntity<TUser, TUserKey>
        where TKey : IEquatable<TKey>
        where TUserKey : IEquatable<TUserKey>
    {
        /// <summary>
        /// Owner of the entity.
        /// </summary>
        public virtual TUser User { get; set; }

        /// <summary>
        /// Owner's id.
        /// </summary>
        public virtual TUserKey UserId { get; set; }
    }
}
