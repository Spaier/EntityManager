using System;

namespace EntityManager
{
    /// <summary>
    /// Base entity with id of type <typeparamref name="TKey"/>.
    /// </summary>
    /// <typeparam name="TKey">The type used for the primary key for the entity.</typeparam>
    public abstract class EntityBase<TKey> : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Id of the entity.
        /// </summary>
        public virtual TKey Id { get; set; }
    }
}
