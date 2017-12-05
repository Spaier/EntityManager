using System;

namespace EntityManager
{
    /// <summary>
    /// Interface for accessing PK.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    public interface IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Id of the Item.
        /// </summary>
        TKey Id { get; set; }
    }
}
