using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using static EntityManager.Benchmarks.NetCore.SampleData;

namespace EntityManager.Benchmarks.NetCore
{
    [RPlotExporter]
    public class GetKeyValuesBenchmark
    {
        [Benchmark]
        public IEnumerable<object> ArrayBenchmark()
        {
            yield return GetKeyValuesWrapper(GetKeyValuesArray, SingleLong);
            yield return GetKeyValuesWrapper(GetKeyValuesArray, SingleString);
            yield return GetKeyValuesWrapper(GetKeyValuesArray, SingleGuid);
            yield return GetKeyValuesWrapper(GetKeyValuesArray, DoubleLong);
            yield return GetKeyValuesWrapper(GetKeyValuesArray, DoubleStringLong);
            yield return GetKeyValuesWrapper(GetKeyValuesArray, DoubleGuidLong);
        }

        [Benchmark]
        public IEnumerable<object> LinqBenchMark()
        {
            yield return GetKeyValuesWrapper(GetKeyValuesLinq, SingleLong);
            yield return GetKeyValuesWrapper(GetKeyValuesLinq, SingleString);
            yield return GetKeyValuesWrapper(GetKeyValuesLinq, SingleGuid);
            yield return GetKeyValuesWrapper(GetKeyValuesLinq, DoubleLong);
            yield return GetKeyValuesWrapper(GetKeyValuesLinq, DoubleStringLong);
            yield return GetKeyValuesWrapper(GetKeyValuesLinq, DoubleGuidLong);
        }

        [Benchmark]
        public IEnumerable<object> PLinqBenchMark()
        {
            yield return GetKeyValuesWrapper(GetKeyValuesPLinq, SingleLong);
            yield return GetKeyValuesWrapper(GetKeyValuesPLinq, SingleString);
            yield return GetKeyValuesWrapper(GetKeyValuesPLinq, SingleGuid);
            yield return GetKeyValuesWrapper(GetKeyValuesPLinq, DoubleLong);
            yield return GetKeyValuesWrapper(GetKeyValuesPLinq, DoubleStringLong);
            yield return GetKeyValuesWrapper(GetKeyValuesPLinq, DoubleGuidLong);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<object> GetKeyValuesWrapper<TEntity>(Func<TEntity, IReadOnlyList<IProperty>, IEnumerable<object>> func,
            TEntity entity)
        {
            return func(entity, Context.GetKeyProperties<TEntity>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object[] GetKeyValuesArray<TEntity>(TEntity entity, IReadOnlyList<IProperty> keyProperties)
        {
            var keyValues = new object[keyProperties.Count];
            for (int i = 0; i < keyValues.Length; i++)
            {
                keyValues[i] = entity.GetType().GetProperty(keyProperties[i].PropertyInfo.Name).GetValue(entity);
            }
            return keyValues;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<object> GetKeyValuesLinq<TEntity>(TEntity entity, IReadOnlyList<IProperty> keyProperties)
        {
            return keyProperties
                .Select(property => entity.GetType().GetProperty(property.PropertyInfo.Name).GetValue(entity));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<object> GetKeyValuesPLinq<TEntity>(TEntity entity, IReadOnlyList<IProperty> keyProperties)
        {
            return keyProperties
                .AsParallel()
                .Select(property => entity.GetType().GetProperty(property.PropertyInfo.Name).GetValue(entity));
        }
    }
}
