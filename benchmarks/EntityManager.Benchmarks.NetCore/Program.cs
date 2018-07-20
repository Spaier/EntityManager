using System;
using BenchmarkDotNet.Running;

namespace EntityManager.Benchmarks.NetCore
{
    internal static class Program
    {
        private static void Main()
        {
            var switcher = new BenchmarkSwitcher(new[]
            {
                typeof(BuildPredicateBenchmark),
                typeof(GetKeyValuesBenchmark),
            });
            switcher.Run();
        }
    }
}
