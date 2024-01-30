using MediatR;
using Microsoft.AspNetCore.Mvc;
using Versus.Core.Features.Weather;

namespace Versus.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IMediator _mediator;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IEnumerable<WeatherForecastDto>> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Entering {Endpoint}", nameof(Get));
        return await _mediator.Send(new GetForecast.Request(), cancellationToken);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Entering {Endpoint} with id {id}", nameof(GetById), id);
        var data = await _mediator.Send(new GetForecastById.Request(id), cancellationToken);

        if (data == null)
        {
            return NotFound();
        }

        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Entering {Endpoint}", nameof(Create));
        var data = await _mediator.Send(new CreateForecast.Request(), cancellationToken);

        return CreatedAtAction(nameof(Create), new { id = data.Id }, data);
    }
}