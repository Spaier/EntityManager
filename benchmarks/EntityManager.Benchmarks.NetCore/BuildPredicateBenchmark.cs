using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using static EntityManager.Benchmarks.NetCore.SampleData;
using static Microsoft.EntityFrameworkCore.EFCoreExtensions;

namespace EntityManager.Benchmarks.NetCore
{
    public class BuildPredicateBenchmark
    {
        [Benchmark]
        public IEnumerable<BinaryExpression> ArrayBenchmark()
        {
            yield return BuildPredicateWrapper(BuildPredicateArray, SingleLong);
            yield return BuildPredicateWrapper(BuildPredicateArray, SingleString);
            yield return BuildPredicateWrapper(BuildPredicateArray, SingleGuid);
            yield return BuildPredicateWrapper(BuildPredicateArray, DoubleLong);
            yield return BuildPredicateWrapper(BuildPredicateArray, DoubleStringLong);
            yield return BuildPredicateWrapper(BuildPredicateArray, DoubleGuidLong);
        }

        [Benchmark]
        public IEnumerable<BinaryExpression> LinqBenchMark()
        {
            yield return BuildPredicateWrapper(BuildPredicateLinq, SingleLong);
            yield return BuildPredicateWrapper(BuildPredicateLinq, SingleString);
            yield return BuildPredicateWrapper(BuildPredicateLinq, SingleGuid);
            yield return BuildPredicateWrapper(BuildPredicateLinq, DoubleLong);
            yield return BuildPredicateWrapper(BuildPredicateLinq, DoubleStringLong);
            yield return BuildPredicateWrapper(BuildPredicateLinq, DoubleGuidLong);
        }

        private BinaryExpression BuildPredicateWrapper<TEntity>(
            Func<IReadOnlyList<IProperty>, ValueBuffer, ParameterExpression, BinaryExpression> func,
            TEntity entity)
        {
            var entityParameter = Expression.Parameter(typeof(TEntity), "e");
            var keyProperties = Context.GetKeyProperties<TEntity>();
            return func(keyProperties, new ValueBuffer(entity.GetKeyValues(keyProperties).ToArray()), entityParameter);
        }

        private static BinaryExpression BuildPredicateArray(IReadOnlyList<IProperty> keyProperties, ValueBuffer keyValues,
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

        private static BinaryExpression BuildPredicateLinq(IReadOnlyList<IProperty> keyProperties, ValueBuffer keyValues,
            ParameterExpression entityParameter)
        {
            var keyValuesConstant = Expression.Constant(keyValues);
            BinaryExpression predicate = null;
            var i = 0;
            var dummy = from property in keyProperties
                        let equalsExpression =
                        Expression.Equal(
                            Expression.Call(
                                _efPropertyMethod.MakeGenericMethod(property.ClrType),
                                entityParameter,
                                Expression.Constant(property.Name, typeof(string))),
                            Expression.Convert(
                                Expression.Call(
                                    keyValuesConstant,
                                    _valueBufferGetValueMethod,
                                    Expression.Constant(i++)),
                                property.ClrType))
                        select predicate = predicate == null ? equalsExpression : Expression.AndAlso(predicate, equalsExpression);
            return predicate;
        }
    }
}
