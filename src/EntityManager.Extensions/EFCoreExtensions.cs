using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Static class containing EFCore extension methods.
    /// </summary>
    public static class EFCoreExtensions
    {
        private static readonly MethodInfo _efPropertyMethod = typeof(EF).GetTypeInfo().GetDeclaredMethod(nameof(EF.Property));

        private static readonly MethodInfo _valueBufferGetValueMethod = typeof(ValueBuffer).GetRuntimeProperties()
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
            => queryable.Any(BuildCheck(context, entity));

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
            => queryable.AnyAsync(BuildCheck(context, entity));

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
            => queryable.AnyAsync(BuildCheck(context, entity), cancellationToken);

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
        public static Task<bool> AnyByPrimaryKeyAsync<TEntity>(this DbContext context, object[] keyValues, CancellationToken cancellationToken)
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
            => queryable.Any(BuildCheck<TEntity>(context, keyValues));

        /// <summary>
        /// Asynchronously determines whether an entity with the given primary key exists in a database.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="queryable"></param>
        /// <param name="context"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static Task<bool> AnyByPrimaryKeyAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext context, params object[] keyValues)
            where TEntity : class
            => queryable.AnyAsync(BuildCheck<TEntity>(context, keyValues));

        /// <summary>
        /// Asynchronously determines whether an entity with the given primary key exists in a database.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="queryable"></param>
        /// <param name="context"></param>
        /// <param name="keyValues"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        public static Task<bool> AnyByPrimaryKeyAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext context, object[] keyValues,
            CancellationToken cancellationToken)
            where TEntity : class
            => queryable.AnyAsync(BuildCheck<TEntity>(context, keyValues), cancellationToken);

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
        public static Task<TEntity> GetByPrimaryKeyAsync<TEntity>(this DbContext context, object[] keyValues, CancellationToken cancellationToken)
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
            => queryable.SingleOrDefault(BuildCheck<TEntity>(context, keyValues));

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
            => queryable.SingleOrDefaultAsync(BuildCheck<TEntity>(context, keyValues));

        /// <summary>
        /// Asynchronously returns an entity with given primary key or a default value if it doesn't exist.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="queryable"></param>
        /// <param name="context"></param>
        /// <param name="keyValues"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        public static Task<TEntity> GetByPrimaryKeyAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext context, object[] keyValues,
            CancellationToken cancellationToken)
            where TEntity : class
            => queryable.SingleOrDefaultAsync(BuildCheck<TEntity>(context, keyValues), cancellationToken);

        #endregion GetByPrimaryKey

        #region GetKeyValues

        /// <summary>
        /// Converts values of a primary key from string[] to clr types and returns them as an object array.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="context">Database.</param>
        /// <param name="stringKeyValues">Primary key.</param>
        /// <returns>Primary key as an object array.</returns>
        /// <exception cref="ArgumentException"><paramref name="stringKeyValues"/> is null or amount of values doesn't match to amount defined in a
        /// primary key</exception>
        public static object[] GetKeyValues<TEntity>(this DbContext context, string[] stringKeyValues)
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
        public static object[] GetKeyValues<TEntity>(this DbContext context, TEntity entity)
            => GetKeyValues(entity, context.GetKeyProperties<TEntity>());

        private static object[] GetKeyValues<TEntity>(TEntity entity, IReadOnlyList<IProperty> keyProperties)
        {
            var keyValues = new object[keyProperties.Count];
            for (int i = 0; i < keyValues.Length; i++)
            {
                keyValues[i] = entity.GetType().GetProperty(keyProperties[i].PropertyInfo.Name).GetValue(entity);
            }
            return keyValues;
        }

        private static (IReadOnlyList<IProperty>, object[]) GetKeyPropertiesAndValues<TEntity>(DbContext context, TEntity entity)
        {
            var keyProperties = context.GetKeyProperties<TEntity>();
            return (keyProperties, GetKeyValues(entity, keyProperties));
        }

        private static IReadOnlyList<IProperty> GetKeyProperties<TEntity>(this DbContext context)
            => (context as IDbContextDependencies)?.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties;

        #endregion GetKeyValues

        private static Expression<Func<TEntity, bool>> BuildCheck<TEntity>(DbContext context, TEntity entity)
        {
            var (keyProperties, keyValues) = GetKeyPropertiesAndValues(context, entity);
            return BuildLambda<TEntity>(keyProperties, new ValueBuffer(keyValues));
        }

        private static Expression<Func<TEntity, bool>> BuildCheck<TEntity>(DbContext context, object[] keyValues) =>
            BuildLambda<TEntity>(GetKeyPropertiesForKeyValues<TEntity>(context, keyValues), new ValueBuffer(keyValues));

        private static IReadOnlyList<IProperty> GetKeyPropertiesForKeyValues<TEntity>(DbContext context, object[] keyValues)
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
                if (keyValues[i] == null) { throw new ArgumentNullException(nameof(keyValues)); }
                var valueType = keyValues[i].GetType();
                var propertyType = keyProperties[i].ClrType;
                if (valueType != propertyType)
                {
                    throw new ArgumentException(
                        CoreStrings.FindValueTypeMismatch(
                            i, typeof(TEntity).ShortDisplayName(), valueType.ShortDisplayName(), propertyType.ShortDisplayName()));
                }
            }
            return keyProperties;
        }

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
