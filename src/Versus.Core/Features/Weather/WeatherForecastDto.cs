namespace Versus.Core.Features.Weather;

public record WeatherForecastDto
{
    public Guid Id { get; init; }

    public DateOnly Date { get; init; }
    public int TemperatureC { get; init; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; init; }
}
