using System;

namespace EntityManager
{
    /// <summary>
    /// Interface for accessing primary key.
    /// </summary>
    /// <typeparam name="TKey">The type used for the primary key for the entity.</typeparam>
    public interface IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Id of the entity.
        /// </summary>
        TKey Id { get; set; }
    }
}
