using MediatR;
using Microsoft.EntityFrameworkCore;
using Versus.Domain;

namespace Versus.Core.Features.Weather;

public class GetForecastById
{
    public record Request(int Id) : IRequest<WeatherForecastDto?>;

    public class RequestHandler : IRequestHandler<Request, WeatherForecastDto?>
    {
        private readonly VersusDbContext _dbContext;

        public RequestHandler(VersusDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<WeatherForecastDto?> Handle(Request request, CancellationToken cancellationToken)
        {
            return await _dbContext.WeatherForecasts
                .Where(x => x.Id == request.Id)
                .Select(x => new WeatherForecastDto
                {
                    Id = x.Id,
                    Date = x.Date,
                    TemperatureC = x.TemperatureC,
                    Summary = x.Summary
                })
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}