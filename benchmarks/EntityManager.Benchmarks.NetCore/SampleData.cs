using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace EntityManager.Benchmarks.NetCore
{
    public static class SampleData
    {
        public class BenchmarkDbContext : DbContext
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

        public static BenchmarkDbContext Context { get; } = new BenchmarkDbContext();

        public static SingleKey<long> SingleLong { get; } = new SingleKey<long>
        {
            Id = 10,
            Name = "AbraKadabra",
            Number = 300
        };

        public static SingleKey<string> SingleString { get; } = new SingleKey<string>
        {
            Id = "somecoolemail@gmail.com",
            Name = "AbraKadabra",
            Number = 300
        };

        public static SingleKey<Guid> SingleGuid { get; } = new SingleKey<Guid>
        {
            Id = Guid.NewGuid(),
            Name = "AbraKadabra",
            Number = 300
        };

        public static DoubleKey<long, long> DoubleLong { get; } = new DoubleKey<long, long>
        {
            Id1 = 1,
            Id2 = 2,
            Name = "AbraKadabra",
            Number = 300
        };

        public static DoubleKey<string, long> DoubleStringLong { get; } = new DoubleKey<string, long>
        {
            Id1 = "somecoolemail@gmail.com",
            Id2 = 1,
            Name = "AbraKadabra",
            Number = 300
        };

        public static DoubleKey<Guid, long> DoubleGuidLong { get; } = new DoubleKey<Guid, long>
        {
            Id1 = Guid.NewGuid(),
            Id2 = 2,
            Name = "AbraKadabra",
            Number = 300
        };
    }
}
