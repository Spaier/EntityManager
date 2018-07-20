using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

[assembly: InternalsVisibleTo("EntityManager.Benchmarks.NetCore")]

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Static class containing EFCore extension methods.
    /// </summary>
    public static class EFCoreExtensions
    {
        internal static readonly MethodInfo _efPropertyMethod = typeof(EF).GetTypeInfo().GetDeclaredMethod(nameof(EF.Property));

        internal static readonly MethodInfo _valueBufferGetValueMethod = typeof(ValueBuffer).GetRuntimeProperties()
            .Single(p => p.GetIndexParameters().Length > 0).GetMethod;

        #region AnyByEntity

        /// <summary>
        /// Determines whether an entity with the primary key of an <paramref name="entity"/> exists in a database.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool AnyByEntity<TEntity>(this DbContext context, TEntity entity)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().AnyByEntity(context, entity);

        /// <summary>
        /// Asynchronously determines whether an entity with the primary key of an <paramref name="entity"/> exists in a database.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Task<bool> AnyByEntityAsync<TEntity>(this DbContext context, TEntity entity)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().AnyByEntityAsync(context, entity);

        /// <summary>
        /// Asynchronously determines whether an entity with the primary key of an <paramref name="entity"/> exists in a database.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        public static Task<bool> AnyByEntityAsync<TEntity>(this DbContext context, TEntity entity, CancellationToken cancellationToken)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().AnyByEntityAsync(context, entity, cancellationToken);

        /// <summary>
        /// Determines whether an entity with the primary key of an <paramref name="entity"/> exists in a database.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="queryable"></param>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool AnyByEntity<TEntity>(this IQueryable<TEntity> queryable, DbContext context, TEntity entity)
            where TEntity : class
            => IsKeyContainsNull(entity, context, out var keyProperties, out var keyValues) ? false
            : queryable.Any(BuildCheck<TEntity>(keyValues, keyProperties));

        /// <summary>
        /// Asynchronously determines whether an entity with the primary key of an <paramref name="entity"/> exists in a database.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="queryable"></param>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Task<bool> AnyByEntityAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext context, TEntity entity)
            where TEntity : class
            => IsKeyContainsNull(entity, context, out var keyProperties, out var keyValues) ? Task.FromResult(false)
            : queryable.AnyAsync(BuildCheck<TEntity>(keyValues, keyProperties));

        /// <summary>
        /// Asynchronously determines whether an entity with the primary key of an <paramref name="entity"/> exists in a database.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="queryable"></param>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        public static Task<bool> AnyByEntityAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext context, TEntity entity,
            CancellationToken cancellationToken)
            where TEntity : class
            => IsKeyContainsNull(entity, context, out var keyProperties, out var keyValues) ? Task.FromResult(false)
            : queryable.AnyAsync(BuildCheck<TEntity>(keyValues, keyProperties), cancellationToken);

        #endregion AnyByEntity

        #region AnyByPrimaryKey

        /// <summary>
        /// Determines whether an entity with the given primary key exists in a database.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="context"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static bool AnyByPrimaryKey<TEntity>(this DbContext context, params object[] keyValues)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().AnyByPrimaryKey(context, keyValues);

        /// <summary>
        /// Asynchronously determines whether an entity with the given primary key exists in a database.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="context"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static Task<bool> AnyByPrimaryKeyAsync<TEntity>(this DbContext context, params object[] keyValues)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().AnyByPrimaryKeyAsync(context, keyValues);

        /// <summary>
        /// Asynchronously determines whether an entity with the given primary key exists in a database.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="context"></param>
        /// <param name="keyValues"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        public static Task<bool> AnyByPrimaryKeyAsync<TEntity>(this DbContext context, object[] keyValues,
            CancellationToken cancellationToken)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().AnyByPrimaryKeyAsync(context, keyValues, cancellationToken);

        /// <summary>
        /// Determines whether an entity with the given primary key exists in a database.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="queryable"></param>
        /// <param name="context"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static bool AnyByPrimaryKey<TEntity>(this IQueryable<TEntity> queryable, DbContext context, params object[] keyValues)
            where TEntity : class
            => IsKeyContainsNull(keyValues) ? false
            : queryable.Any(BuildCheck<TEntity>(context, keyValues));

        /// <summary>
        /// Asynchronously determines whether an entity with the given primary key exists in a database.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="queryable"></param>
        /// <param name="context"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static Task<bool> AnyByPrimaryKeyAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext context,
            params object[] keyValues)
            where TEntity : class
            => IsKeyContainsNull(keyValues) ? Task.FromResult(false)
            : queryable.AnyAsync(BuildCheck<TEntity>(context, keyValues));

        /// <summary>
        /// Asynchronously determines whether an entity with the given primary key exists in a database.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="queryable"></param>
        /// <param name="context"></param>
        /// <param name="keyValues"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        public static Task<bool> AnyByPrimaryKeyAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext context,
            object[] keyValues, CancellationToken cancellationToken)
            where TEntity : class
            => IsKeyContainsNull(keyValues) ? Task.FromResult(false)
            : queryable.AnyAsync(BuildCheck<TEntity>(context, keyValues), cancellationToken);

        #endregion AnyByPrimaryKey

        #region GetByPrimaryKey

        /// <summary>
        /// Return the entity with given primary key as no tracking or a default value if it doesn't exist.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="context">Database.</param>
        /// <param name="keyValues">Primary key.</param>
        /// <returns>An entity or a default value.</returns>
        public static TEntity GetByPrimaryKey<TEntity>(this DbContext context, params object[] keyValues)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().GetByPrimaryKey(context, keyValues);

        /// <summary>
        /// Asynchronously returns the entity with given primary key as no tracking or a default value if it doesn't exist.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="context"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static Task<TEntity> GetByPrimaryKeyAsync<TEntity>(this DbContext context, params object[] keyValues)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().GetByPrimaryKeyAsync(context, keyValues);

        /// <summary>
        /// Asynchronously returns the entity with given primary key as no tracking or a default value if it doesn't exist.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="context"></param>
        /// <param name="keyValues"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        public static Task<TEntity> GetByPrimaryKeyAsync<TEntity>(this DbContext context, object[] keyValues,
            CancellationToken cancellationToken)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().GetByPrimaryKeyAsync(context, keyValues, cancellationToken);

        /// <summary>
        /// Returns an entity with given primary key or a default value if it doesn't exist.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="queryable"></param>
        /// <param name="context"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static TEntity GetByPrimaryKey<TEntity>(this IQueryable<TEntity> queryable, DbContext context, params object[] keyValues)
            where TEntity : class
            => IsKeyContainsNull(keyValues) ? null
            : queryable.SingleOrDefault(BuildCheck<TEntity>(context, keyValues));

        /// <summary>
        /// Asynchronously returns an entity with given primary key or a default value if it doesn't exist.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="queryable"></param>
        /// <param name="context"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static Task<TEntity> GetByPrimaryKeyAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext context,
            params object[] keyValues)
            where TEntity : class
            => IsKeyContainsNull(keyValues) ? Task.FromResult<TEntity>(null)
            : queryable.SingleOrDefaultAsync(BuildCheck<TEntity>(context, keyValues));

        /// <summary>
        /// Asynchronously returns an entity with given primary key or a default value if it doesn't exist.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="queryable"></param>
        /// <param name="context"></param>
        /// <param name="keyValues"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        public static Task<TEntity> GetByPrimaryKeyAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext context,
            object[] keyValues, CancellationToken cancellationToken)
            where TEntity : class
            => IsKeyContainsNull(keyValues) ? Task.FromResult<TEntity>(null)
            : queryable.SingleOrDefaultAsync(BuildCheck<TEntity>(context, keyValues), cancellationToken);

        #endregion GetByPrimaryKey

        /// <summary>
        /// Converts values of a primary key from string[] to clr types and returns them as an object array.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="context">Database.</param>
        /// <param name="stringKeyValues">Primary key.</param>
        /// <returns>Primary key as an object array.</returns>
        /// <exception cref="ArgumentException"><paramref name="stringKeyValues"/> is null or amount of values doesn't match to amount defined in a
        /// primary key</exception>
        public static object[] GetKeyValues<TEntity>(this DbContext context, params string[] stringKeyValues)
        {
            if (stringKeyValues == null || stringKeyValues.Length == 0) { throw new ArgumentException(nameof(stringKeyValues)); }
            var keyProperties = context.GetKeyProperties<TEntity>();
            if (stringKeyValues.Length != keyProperties.Count) { throw new ArgumentException(nameof(stringKeyValues)); }
            var array = new object[stringKeyValues.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Convert.ChangeType(stringKeyValues[i], keyProperties[i].ClrType);
            }
            return array;
        }

        /// <summary>
        /// Returns primary key of an entity as an object array.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="context">Database.</param>
        /// <param name="entity">Entity.</param>
        /// <returns>Primary key as an object array.</returns>
        public static IEnumerable<object> GetKeyValues<TEntity>(this DbContext context, TEntity entity)
            => GetKeyValues(entity, context.GetKeyProperties<TEntity>());

        internal static IEnumerable<object> GetKeyValues<TEntity>(this TEntity entity, IReadOnlyList<IProperty> keyProperties)
        {
            return keyProperties
                .Select(property => entity.GetType().GetProperty(property.PropertyInfo.Name).GetValue(entity));
        }

        private static IReadOnlyList<IProperty> CheckKeyProperties<TEntity>(this DbContext context, object[] keyValues)
        {
            var keyProperties = GetKeyProperties<TEntity>(context);
            if (keyProperties.Count != keyValues.Length)
            {
                if (keyProperties.Count == 1)
                {
                    throw new ArgumentException(
                        CoreStrings.FindNotCompositeKey(typeof(TEntity).ShortDisplayName(), keyValues.Length));
                }
                throw new ArgumentException(
                    CoreStrings.FindValueCountMismatch(typeof(TEntity).ShortDisplayName(), keyProperties.Count, keyValues.Length));
            }
            for (var i = 0; i < keyValues.Length; i++)
            {
                var valueType = keyValues[i].GetType();
                var propertyType = keyProperties[i].ClrType;
                if (valueType != propertyType.UnwrapNullableType())
                {
                    throw new ArgumentException(
                        CoreStrings.FindValueTypeMismatch(
                            i, typeof(TEntity).ShortDisplayName(), valueType.ShortDisplayName(), propertyType.ShortDisplayName()));
                }
            }
            return keyProperties;
        }

#pragma warning disable RCS1202

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IReadOnlyList<IProperty> GetKeyProperties<TEntity>(this DbContext context)
            => (context as IDbContextDependencies).Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties;

#pragma warning restore RCS1202

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsKeyContainsNull(IEnumerable<object> keyValues)
        {
            return keyValues?.Any(it => it == null) != false;
        }

        private static bool IsKeyContainsNull<TEntity>(TEntity entity, DbContext context,
            out IReadOnlyList<IProperty> keyProperties, out IEnumerable<object> keyValues)
            where TEntity : class
        {
            if (entity == null)
            {
                keyProperties = null;
                keyValues = null;
                return true;
            }
            else
            {
                return IsKeyContainsNull(keyValues = GetKeyValues(entity, keyProperties = GetKeyProperties<TEntity>(context)));
            }
        }

        private static Expression<Func<TEntity, bool>> BuildCheck<TEntity>(this DbContext context, object[] keyValues)
            => BuildLambda<TEntity>(context.CheckKeyProperties<TEntity>(keyValues), new ValueBuffer(keyValues));

        private static Expression<Func<TEntity, bool>> BuildCheck<TEntity>(this IEnumerable<object> keyValues,
            IReadOnlyList<IProperty> keyProperties)
            => BuildLambda<TEntity>(keyProperties, new ValueBuffer(keyValues.ToArray()));

        private static Expression<Func<TEntity, bool>> BuildLambda<TEntity>(IReadOnlyList<IProperty> keyProperties, ValueBuffer keyValues)
        {
            var entityParameter = Expression.Parameter(typeof(TEntity), "e");
            return Expression.Lambda<Func<TEntity, bool>>(BuildPredicate(keyProperties, keyValues, entityParameter), entityParameter);
        }

        private static BinaryExpression BuildPredicate(IReadOnlyList<IProperty> keyProperties, ValueBuffer keyValues,
            ParameterExpression entityParameter)
        {
            var keyValuesConstant = Expression.Constant(keyValues);
            BinaryExpression predicate = null;
            for (var i = 0; i < keyProperties.Count; i++)
            {
                var property = keyProperties[i];
                var equalsExpression =
                    Expression.Equal(
                        Expression.Call(
                            _efPropertyMethod.MakeGenericMethod(property.ClrType),
                            entityParameter,
                            Expression.Constant(property.Name, typeof(string))),
                        Expression.Convert(
                            Expression.Call(
                                keyValuesConstant,
                                _valueBufferGetValueMethod,
                                Expression.Constant(i)),
                            property.ClrType));
                predicate = predicate == null ? equalsExpression : Expression.AndAlso(predicate, equalsExpression);
            }
            return predicate;
        }
    }
}
