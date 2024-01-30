using MediatR;
using Versus.Domain;
using Versus.Domain.Entities;

namespace Versus.Core.Features.Weather;

public static class CreateForecast
{
    public record Request : IRequest<WeatherForecast>;

    public class RequestHandler : IRequestHandler<Request, WeatherForecast>
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        private readonly VersusDbContext _dbContext;

        public RequestHandler(VersusDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<WeatherForecast> Handle(Request request, CancellationToken cancellationToken)
        {
            var forecast = new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };

            await _dbContext.WeatherForecasts.AddAsync(forecast, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return forecast;
        }
    }
}