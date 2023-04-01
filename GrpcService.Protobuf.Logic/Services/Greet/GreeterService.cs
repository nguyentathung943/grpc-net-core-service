using Grpc.Core;
using Microsoft.Extensions.Logging;
using GrpcGreeterProtobuf;

namespace GrpcService.Protobuf.Logic.Services.GreeterService
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = $"Hello {request.Name} How are you doing today? I hope you're doing well. It's great to see you again. I hope you're having a fantastic day so far. I'm excited to chat with you today and catch up on what's been going on in your life. Let me know if there's anything new or exciting that you'd like to share with me. I'm always here to listen and support you in any way that I can. Have a wonderful day! blush"
            }
            );
        }
    }
}
