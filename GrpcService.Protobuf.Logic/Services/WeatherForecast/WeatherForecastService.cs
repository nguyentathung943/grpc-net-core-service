using Grpc.Core;
using GrpcWeatherForecastProtobuf;

namespace GrpcService.Protobuf.Services.WeatherForecastService
{
    public class WeatherForecastService : WeatherForecast.WeatherForecastBase
    {
        public WeatherForecastService()
        {}

        public override Task<WeatherForecastResponse> Forecasting(WeatherForecastRequest request, ServerCallContext context)
        {
            var response = new WeatherForecastResponse();
            var forecastData = new List<WeatherForecastData>();
            var limit = request.DataLimit;

            for (int i = 0; i < limit; i++)
            {
                var tempC = Random.Shared.Next(-20, 55);
                forecastData.Add(new()
                {
                    Timestamp = DateTime.Now.ToString(),
                    Summary = $"Forecast {i}",
                    TemperatureC = tempC,
                    TemperatureF = 32 + (int)(tempC / 0.5556)
                });
            }

            response.WeatherForecastData.AddRange(forecastData);

            return Task.FromResult(response);
        }

        public override async Task ForecastStreaming(WeatherForecastRequest request, IServerStreamWriter<WeatherForecastData> responseStream, ServerCallContext context)
        {
            var amount = request.DataLimit;
            var count = 0;
            var tempC = 0;

            for (count = 0; count < amount; count++)
            {
                tempC = Random.Shared.Next(-20, 55);

                await responseStream.WriteAsync(new()
                {
                    Timestamp = DateTime.Now.ToString(),
                    Summary = $"Forecast {count}",
                    TemperatureC = tempC,
                    TemperatureF = 32 + (int)(tempC / 0.5556)
                });

                await Task.Delay(100);
            }

        }

        public override async Task ForecastingWithChunkLimit(WeatherForecastRequest request, IServerStreamWriter<WeatherForecastResponse> responseStream, ServerCallContext context)
        {
            var response = new WeatherForecastResponse();
            var forecastData = new List<WeatherForecastData>();
            var limitPerChunk = request.ChunkLimit;
            var amount = request.DataLimit;
            var limitCount = 0;
            var count = 0;
            var tempC = 0;


            for (count = 0; count < amount; count++)
            {
                if (limitCount == limitPerChunk)
                {
                    response.WeatherForecastData.AddRange(forecastData);
                    await responseStream.WriteAsync(response);
                    forecastData = new List<WeatherForecastData>();
                    response = new WeatherForecastResponse();
                    limitCount = 0;
                    await Task.Delay(200);
                }

                tempC = Random.Shared.Next(-20, 55);
                forecastData.Add(new()
                {
                    Timestamp = DateTime.Now.ToString(),
                    Summary = $"Forecast {count}",
                    TemperatureC = tempC,
                    TemperatureF = 32 + (int)(tempC / 0.5556)
                });

                limitCount++;
            }

            // Send the rest if there is any left
            if (forecastData.Any())
            {
                response.WeatherForecastData.AddRange(forecastData);
                await responseStream.WriteAsync(response);
                await Task.Delay(200);
            }

        }
    }
}
