using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Lambda.Model;

namespace ColdStartMeasure
{
    public class Benchmark
    {
        private const int NumberOfInvocations = 5;

        private readonly string _functionName;
        private readonly IAmazonLambda _lambdaClient;

        public Benchmark(string functionName)
        {
            _functionName = functionName;
            _lambdaClient = new AmazonLambdaClient();
        }

        public async Task<decimal> MeasureAverageColdStartAsync(int memorySize)
        {
            await SetLambdaMemorySizeAsync(memorySize);

            var measurements = new List<decimal>();
            for (var i = 0; i < NumberOfInvocations; i++)
            {
                var measurement = await MeasureSingleColdStartAsync();
                measurements.Add(measurement);
            }

            return measurements.Average();
        }

        private async Task<decimal> MeasureSingleColdStartAsync()
        {
            await ForceColdStartOnNextInvocation();

            var coldInvocationTime = await MeasureSingleInvocationAsync();
            var warmInvocationTime = await MeasureSingleInvocationAsync();

            return coldInvocationTime - warmInvocationTime;
        }

        private async Task SetLambdaMemorySizeAsync(int memorySize)
            => await _lambdaClient.UpdateFunctionConfigurationAsync(
                   new UpdateFunctionConfigurationRequest
                   {
                       FunctionName = _functionName,
                       MemorySize = memorySize
                   }
               );

        private async Task ForceColdStartOnNextInvocation()
            => await _lambdaClient.UpdateFunctionConfigurationAsync(
                   new UpdateFunctionConfigurationRequest
                   {
                       FunctionName = _functionName,
                       Description = Guid.NewGuid().ToString()
                   }
               );

        private async Task<decimal> MeasureSingleInvocationAsync()
        {
            var sw = new Stopwatch();
            sw.Start();

            var result = await _lambdaClient.InvokeAsync(
                             new InvokeRequest
                             {
                                 FunctionName = _functionName,
                                 InvocationType = InvocationType.RequestResponse,
                                 Payload = "{}"
                             }
                         );

            sw.Stop();

            return sw.ElapsedMilliseconds / 1000.0m;
        }
    }
}
