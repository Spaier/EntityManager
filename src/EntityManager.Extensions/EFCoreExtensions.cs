using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Static class containing EFCore extension methods.
    /// </summary>
    public static class EFCoreExtensions
    {
        private static readonly MethodInfo _efPropertyMethod = typeof(EF).GetTypeInfo().GetDeclaredMethod(nameof(EF.Property));

        private static readonly MethodInfo _valueBufferGetValueMethod = typeof(ValueBuffer).GetRuntimeProperties()
            .Single(p => p.GetIndexParameters().Any()).GetMethod;

        #region AnyByEntity

        public static bool AnyByEntity<TEntity>(this DbContext context, TEntity entity)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().AnyByEntity(context, entity);

        public static Task<bool> AnyByEntityAsync<TEntity>(this DbContext context, TEntity entity)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().AnyByEntityAsync(context, entity);

        public static Task<bool> AnyAsyncEntity<TEntity>(this DbContext context, TEntity entity, CancellationToken cancellationToken)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().AnyByEntityAsync(context, entity, cancellationToken);

        public static bool AnyByEntity<TEntity>(this IQueryable<TEntity> queryable, DbContext context, TEntity entity)
            where TEntity : class
            => queryable.Any(BuildCheck(context, entity));

        public static Task<bool> AnyByEntityAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext context, TEntity entity)
            where TEntity : class
            => queryable.AnyAsync(BuildCheck(context, entity));

        public static Task<bool> AnyByEntityAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext context, TEntity entity,
            CancellationToken cancellationToken)
            where TEntity : class
            => queryable.AnyAsync(BuildCheck(context, entity), cancellationToken);

        #endregion AnyByEntity

        #region AnyByPrimaryKey

        public static bool AnyByPrimaryKey<TEntity>(this DbContext context, params object[] keyValues)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().AnyByPrimaryKey(context, keyValues);

        public static Task<bool> AnyByPrimaryKeyAsync<TEntity>(this DbContext context, params object[] keyValues)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().AnyByPrimaryKeyAsync(context, keyValues);

        public static Task<bool> AnyByPrimaryKeyAsync<TEntity>(this DbContext context, object[] keyValues, CancellationToken cancellationToken)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().AnyByPrimaryKeyAsync(context, keyValues, cancellationToken);

        public static bool AnyByPrimaryKey<TEntity>(this IQueryable<TEntity> queryable, DbContext context, params object[] keyValues)
            where TEntity : class
            => queryable.Any(BuildCheck<TEntity>(context, keyValues));

        public static Task<bool> AnyByPrimaryKeyAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext context, params object[] keyValues)
            where TEntity : class
            => queryable.AnyAsync(BuildCheck<TEntity>(context, keyValues));

        public static Task<bool> AnyByPrimaryKeyAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext context, object[] keyValues,
            CancellationToken cancellationToken)
            where TEntity : class
            => queryable.AnyAsync(BuildCheck<TEntity>(context, keyValues), cancellationToken);

        #endregion AnyByPrimaryKey

        #region GetByPrimaryKey

        public static TEntity GetByPrimaryKey<TEntity>(this DbContext context, params object[] keyValues)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().GetByPrimaryKey(context, keyValues);

        public static Task<TEntity> GetByPrimaryKeyAsync<TEntity>(this DbContext context, params object[] keyValues)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().GetByPrimaryKeyAsync(context, keyValues);

        public static Task<TEntity> GetByPrimaryKeyAsync<TEntity>(this DbContext context, object[] keyValues, CancellationToken cancellationToken)
            where TEntity : class
            => context.Set<TEntity>().AsNoTracking().GetByPrimaryKeyAsync(context, keyValues, cancellationToken);

        public static TEntity GetByPrimaryKey<TEntity>(this IQueryable<TEntity> queryable, DbContext context, params object[] keyValues)
            where TEntity : class
            => queryable.SingleOrDefault(BuildCheck<TEntity>(context, keyValues));

        public static Task<TEntity> GetByPrimaryKeyAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext context,
            params object[] keyValues)
            where TEntity : class
            => queryable.SingleOrDefaultAsync(BuildCheck<TEntity>(context, keyValues));

        public static Task<TEntity> GetByPrimaryKeyAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext context, object[] keyValues,
            CancellationToken cancellationToken)
            where TEntity : class
            => queryable.SingleOrDefaultAsync(BuildCheck<TEntity>(context, keyValues), cancellationToken);

        #endregion GetByPrimaryKey

        /// <summary>
        /// Converts values from string to clr.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="stringKeyValues"></param>
        /// <returns></returns>
        public static object[] GetKeyValues<TEntity>(this DbContext context, string[] stringKeyValues)
        {
            if (stringKeyValues == null || stringKeyValues.Length == 0) { throw new ArgumentException(nameof(stringKeyValues)); }
            var keyProperties = context.GetKeyProperties<TEntity>();
            if (stringKeyValues.Length != keyProperties.Count) { throw new ArgumentException(nameof(stringKeyValues)); }
            return Enumerable
                .Range(0, stringKeyValues.Length)
                .Aggregate(new List<object>(), (list, i) =>
                {
                    var keyType = keyProperties[i].ClrType;
                    var keyValue = Convert.ChangeType(stringKeyValues[i], keyType);
                    list.Add(keyValue);
                    return list;
                }).ToArray();
        }

        /// <summary>
        /// Gets key properties of entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static IReadOnlyList<IProperty> GetKeyProperties<TEntity>(this DbContext context)
            => (context as IDbContextDependencies).Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties;

        private static Expression<Func<TEntity, bool>> BuildCheck<TEntity>(DbContext context, TEntity entity)
        {
            var (keyProperties, keyValues) = GetKeyPropertiesAndValues(context, entity);
            return BuildLambda<TEntity>(keyProperties, new ValueBuffer(keyValues));
        }

        private static Expression<Func<TEntity, bool>> BuildCheck<TEntity>(DbContext context, object[] keyValues) =>
            BuildLambda<TEntity>(GetKeyPropertiesForKeyValues<TEntity>(context, keyValues), new ValueBuffer(keyValues));

        private static (IReadOnlyList<IProperty>, object[]) GetKeyPropertiesAndValues<TEntity>(DbContext context, TEntity entity)
        {
            var keyProperties = context.GetKeyProperties<TEntity>();
            var keyValues = new List<object>();
            foreach (var property in keyProperties) { keyValues.Add(entity.GetType().GetProperty(property.PropertyInfo.Name).GetValue(entity)); }
            return (keyProperties, keyValues.ToArray());
        }

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
