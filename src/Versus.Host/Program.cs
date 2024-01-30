using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Versus.Api.Controllers;
using Versus.Core.Features.Weather;
using Versus.Domain;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddDbContext<VersusDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var coreAssembly = typeof(GetForecast).Assembly;
builder.Services.AddMediatR(options =>
    options.RegisterServicesFromAssembly(coreAssembly));

var apiAssembly = typeof(WeatherForecastController).Assembly;
builder.Services.AddControllers().AddApplicationPart(apiAssembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(options => { options.AddSerilog(); });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();