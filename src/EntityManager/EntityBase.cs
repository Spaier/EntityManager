using System;

namespace EntityManager
{
    /// <summary>
    /// Base Item with ID of type <typeparamref name="TKey"/>.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    public abstract class EntityBase<TKey> : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Id of the item.
        /// </summary>
        public virtual TKey Id { get; set; }
    }
}
