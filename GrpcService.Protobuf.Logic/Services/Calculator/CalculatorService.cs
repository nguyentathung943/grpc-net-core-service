using Grpc.Core;
using Microsoft.Extensions.Logging;
using GrpcCalculatorProtobuf;
using GrpcSumProtobuf;
using GrpcAverageProtobuf;

namespace GrpcService.Protobuf.Logic.Services.CalculatorService
{
    public class CalculatorService : Calculator.CalculatorBase
    {
        private readonly ILogger<CalculatorService> _logger;
        public CalculatorService(ILogger<CalculatorService> logger)
        {
            _logger = logger;
        }

        // Unary - Same as Greeter
        public override Task<SumResponse> Sum(SumRequest request, ServerCallContext context)
        {
            return Task.FromResult(new SumResponse
            {
                Result = request.FirstNumber + request.SecondNumber
            });
        }

        // Client Streaming
        public override async Task<AverageResponse> FindAverage(IAsyncStreamReader<AverageRequest> requestStream, ServerCallContext context)
        {
            int sum = 0;
            int count = 0;


            await foreach (var message in requestStream.ReadAllAsync())
            {
                sum += requestStream.Current.Value;
                count++;
            }

            return new AverageResponse() { Result = (float)(sum) / count };
        }

        // Bi-directional Streaming
        public override async Task FindCurrentMax(IAsyncStreamReader<GrpcMaxProtobuf.MaxRequest> requestStream, IServerStreamWriter<GrpcMaxProtobuf.MaxResponse> responseStream, ServerCallContext context)
        {

            int currentMaxValue = int.MinValue;

            await foreach (var message in requestStream.ReadAllAsync())
            {
                var value = requestStream.Current.Value;

                if (value > currentMaxValue)
                {
                    currentMaxValue = value;
                }

                await responseStream.WriteAsync(new GrpcMaxProtobuf.MaxResponse() { CurrentMaxValue = currentMaxValue, CurrentCompareValue = value });
                await Task.Delay(100);
            }
        }
    }
}
