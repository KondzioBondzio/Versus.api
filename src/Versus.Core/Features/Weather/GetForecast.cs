using MediatR;
using Microsoft.EntityFrameworkCore;
using Versus.Domain;

namespace Versus.Core.Features.Weather;

public static class GetForecast
{
    public record Request : IRequest<IEnumerable<WeatherForecastDto>>;

    public class RequestHandler : IRequestHandler<Request, IEnumerable<WeatherForecastDto>>
    {
        private readonly VersusDbContext _dbContext;

        public RequestHandler(VersusDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<WeatherForecastDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            return await _dbContext.WeatherForecasts
                .Select(x => new WeatherForecastDto
                {
                    Id = x.Id,
                    Date = x.Date,
                    TemperatureC = x.TemperatureC,
                    Summary = x.Summary
                })
                .ToListAsync(cancellationToken);
        }
    }
}