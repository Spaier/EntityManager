using System;
using BenchmarkDotNet.Running;

namespace EntityManager.Benchmarks.NetCore
{
    internal static class Program
    {
        private static void Main()
        {
            var summary = BenchmarkRunner.Run<GetKeyValuesBenchmark>();
            Console.ReadKey();
        }
    }
}
