using Grpc.Core;
using Microsoft.Extensions.Logging;
using GrpcCalculatorProtobuf;
using GrpcSumProtobuf;

namespace GrpcService.Protobuf.Logic.Services.CalculatorService
{
    public class CalculatorService : Calculator.CalculatorBase
    {
        private readonly ILogger<CalculatorService> _logger;
        public CalculatorService(ILogger<CalculatorService> logger)
        {
            _logger = logger;
        }

        public override Task<SumResponse> Sum(SumRequest request, ServerCallContext context)
        {
            return Task.FromResult(new SumResponse
            {
                Result = request.FirstNumber + request.SecondNumber
            });
        }
    }
}
