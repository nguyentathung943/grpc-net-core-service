using Grpc.Core;
using GrpcSumProtobuf;
using GrpcAverageProtobuf;
using GrpcPrimeFactorsProtobuf;
using GrpcCalculatorProtobuf;

namespace GrpcService.Protobuf.Services.CalculatorService
{
    public class CalculatorService : Calculator.CalculatorBase
    {
        private readonly ICalculation _service;

        public CalculatorService(ICalculation service)
        {
            _service = service;
        }

        // Unary - Same as Greeter
        public override Task<SumResponse> Sum(SumRequest request, ServerCallContext context)
        {
            var sumResult = _service.Sum(request.FirstNumber, request.SecondNumber);

            return Task.FromResult(
                new SumResponse
                {
                    Result = sumResult
                }
            );
        }

        //Server Streaming
        public override async Task FindPrimeFactors(PrimesRequest request, IServerStreamWriter<PrimesResponse> responseStream, ServerCallContext context)
        {
            var value = request.Number;
            var divisor = 2;

            while (value > 1)
            {
                if (value % divisor == 0)
                {
                    await responseStream.WriteAsync(new PrimesResponse { Result = divisor });
                    await Task.Delay(100);
                    value = value / divisor;
                }
                else
                {
                    ++divisor;
                }
            }
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

            return new AverageResponse() { Result = _service.DivideFloat(sum, count)};
        }

        // Bi-directional Streaming
        public override async Task FindCurrentMax(IAsyncStreamReader<GrpcMaxProtobuf.MaxRequest> requestStream, IServerStreamWriter<GrpcMaxProtobuf.MaxResponse> responseStream, ServerCallContext context)
        {
            int currentMaxValue = int.MinValue;

            await foreach (var message in requestStream.ReadAllAsync())
            {
                var value = requestStream.Current.Value;

                if (_service.IsGreaterThan(value, currentMaxValue))
                {
                    currentMaxValue = value;
                }

                await responseStream.WriteAsync(new GrpcMaxProtobuf.MaxResponse() { CurrentMaxValue = currentMaxValue, CurrentCompareValue = value });
                await Task.Delay(100);
            }
        }
    }
}
