using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityManager.Benchmarks.NetCore
{
    [RPlotExporter]
    public class GetKeyValuesBenchmark
    {
        public class GetKeyValuesDbContext : DbContext
        {
            public DbSet<SingleKey<long>> Longs { get; set; }
            public DbSet<SingleKey<string>> Strings { get; set; }
            public DbSet<SingleKey<Guid>> Guids { get; set; }
            public DbSet<DoubleKey<long, long>> LongLongs { get; set; }
            public DbSet<DoubleKey<string, long>> StringLongs { get; set; }
            public DbSet<DoubleKey<Guid, long>> GuidLongs { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("benchmark");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<DoubleKey<long, long>>()
                    .HasKey(entity => new { entity.Id1, entity.Id2 });
                modelBuilder.Entity<DoubleKey<string, long>>()
                    .HasKey(entity => new { entity.Id1, entity.Id2 });
                modelBuilder.Entity<DoubleKey<Guid, long>>()
                    .HasKey(entity => new { entity.Id1, entity.Id2 });
            }
        }

        public class SingleKey<TKey>
        {
            public TKey Id { get; set; }
            public string Name { get; set; }
            public long Number { get; set; }
        }

        public class DoubleKey<TKey1, TKey2>
        {
            public TKey1 Id1 { get; set; }
            public TKey2 Id2 { get; set; }
            public string Name { get; set; }
            public long Number { get; set; }
        }

        private readonly GetKeyValuesDbContext _context = new GetKeyValuesDbContext();

        private readonly SingleKey<long> _singleLong = new SingleKey<long>
        {
            Id = 10, Name = "AbraKadabra", Number = 300
        };

        private readonly SingleKey<string> _singleString = new SingleKey<string>
        {
            Id = "somecoolemail@gmail.com", Name = "AbraKadabra", Number = 300
        };

        private readonly SingleKey<Guid> _singleGuid = new SingleKey<Guid>
        {
            Id = Guid.NewGuid(), Name = "AbraKadabra", Number = 300
        };

        private readonly DoubleKey<long, long> _doubleLong = new DoubleKey<long, long>
        {
            Id1 = 1, Id2 = 2, Name = "AbraKadabra", Number = 300
        };

        private readonly DoubleKey<string, long> _doubleStringLong = new DoubleKey<string, long>
        {
            Id1 = "somecoolemail@gmail.com",
            Id2 = 1,
            Name = "AbraKadabra",
            Number = 300
        };

        private readonly DoubleKey<Guid, long> _doubleGuidLong = new DoubleKey<Guid, long>
        {
            Id1 = Guid.NewGuid(),
            Id2 = 2,
            Name = "AbraKadabra",
            Number = 300
        };

        [Benchmark]
        public IEnumerable<object> ArrayBenchmark()
        {
            yield return GetKeyValuesWrapper(GetKeyValuesArray, _singleLong);
            yield return GetKeyValuesWrapper(GetKeyValuesArray, _singleString);
            yield return GetKeyValuesWrapper(GetKeyValuesArray, _singleGuid);
            yield return GetKeyValuesWrapper(GetKeyValuesArray, _doubleLong);
            yield return GetKeyValuesWrapper(GetKeyValuesArray, _doubleStringLong);
            yield return GetKeyValuesWrapper(GetKeyValuesArray, _doubleGuidLong);
        }

        [Benchmark]
        public IEnumerable<object> LinqBenchMark()
        {
            yield return GetKeyValuesWrapper(GetKeyValuesLinq, _singleLong);
            yield return GetKeyValuesWrapper(GetKeyValuesLinq, _singleString);
            yield return GetKeyValuesWrapper(GetKeyValuesLinq, _singleGuid);
            yield return GetKeyValuesWrapper(GetKeyValuesLinq, _doubleLong);
            yield return GetKeyValuesWrapper(GetKeyValuesLinq, _doubleStringLong);
            yield return GetKeyValuesWrapper(GetKeyValuesLinq, _doubleGuidLong);
        }

        [Benchmark]
        public IEnumerable<object> PLinqBenchMark()
        {
            yield return GetKeyValuesWrapper(GetKeyValuesPLinq, _singleLong);
            yield return GetKeyValuesWrapper(GetKeyValuesPLinq, _singleString);
            yield return GetKeyValuesWrapper(GetKeyValuesPLinq, _singleGuid);
            yield return GetKeyValuesWrapper(GetKeyValuesPLinq, _doubleLong);
            yield return GetKeyValuesWrapper(GetKeyValuesPLinq, _doubleStringLong);
            yield return GetKeyValuesWrapper(GetKeyValuesPLinq, _doubleGuidLong);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<object> GetKeyValuesWrapper<TEntity>(Func<TEntity, IReadOnlyList<IProperty>, IEnumerable<object>> func,
            TEntity entity)
        {
            return func(entity, _context.GetKeyProperties<TEntity>());
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
