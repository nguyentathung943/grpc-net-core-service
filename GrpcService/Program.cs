using GrpcService.Protobuf.Logic.Services.CalculatorService;
using GrpcService.Protobuf.Logic.Services.GreeterService;
using GrpcService.Protobuf.Logic.Services.WeatherForecastService;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc(options =>
{
    options.MaxReceiveMessageSize = 100 * 1024 * 1024; // 100 MB
    options.MaxSendMessageSize = 100 * 1024 * 1024; // 100 MB
});

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

builder.Services.AddGrpcReflection();

var app = builder.Build();

app.UseRouting();

// Is required for client calling
app.UseGrpcWeb();

app.UseCors();
//

app.UseEndpoints(endpoints =>
{
    if (app.Environment.IsDevelopment())
    {
        app.MapGrpcReflectionService();
    }

    endpoints.MapGrpcService<GreeterService>().EnableGrpcWeb().RequireCors("AllowAll");
    endpoints.MapGrpcService<CalculatorService>().EnableGrpcWeb().RequireCors("AllowAll");
    endpoints.MapGrpcService<WeatherForecastService>().EnableGrpcWeb().RequireCors("AllowAll");
});


app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
