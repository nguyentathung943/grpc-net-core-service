using Grpc.Core;
using GrpcGreeterProtobuf;
using GrpcWeatherForecastProtobuf;
using Microsoft.Extensions.Logging;

namespace GrpcService.Protobuf.Logic.Services.WeatherForecastService
{
    public class WeatherForecastService: WeatherForecast.WeatherForecastBase
    {

        private readonly ILogger<WeatherForecastService> _logger;
        public WeatherForecastService(ILogger<WeatherForecastService> logger)
        {
            _logger = logger;
        }

        public override Task<WeatherForecastResponse> Forecasting(WeatherForecastRequest request, ServerCallContext context)
        {
            var response = new WeatherForecastResponse();
            var forecastData = new List<WeatherForecastData>();

            for (int i = 0; i < 100000; i++)
            {
                var tempC = Random.Shared.Next(-20, 55);
                forecastData.Add(new() { 
                    Timestamp = DateTime.Now.ToString(),
                    Summary = $"Forecast {i}",
                    TemperatureC = tempC,
                    TemperatureF = 32 + (int)(tempC / 0.5556)
                });
            }

            response.WeatherForecastData.AddRange(forecastData);

            return Task.FromResult(response);
        }
    }
}