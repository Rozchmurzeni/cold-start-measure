using System;
using System.Threading.Tasks;

namespace ColdStartMeasure
{
    class Program
    {
        private static readonly int[] MemorySizes = { 128, 256, 512, 1024, 2048, 3008 };
        
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: Rozchmurzeni.ColdStartMeasure.exe <lambda_function_name>");
                return;
            }
            
            var functionName = args[0];
            var measurement = new Benchmark(functionName);

            Console.WriteLine($"Measuring cold start for function {functionName}.");
            
            foreach (var memorySize in MemorySizes)
            {
                var coldStartTime = await measurement.MeasureAverageColdStartAsync(memorySize);
                
                Console.WriteLine($"{memorySize} MB: {coldStartTime} s");
            }
        }
    }
}
