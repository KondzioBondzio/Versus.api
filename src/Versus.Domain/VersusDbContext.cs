using Microsoft.EntityFrameworkCore;
using Versus.Domain.Entities;

namespace Versus.Domain;

public class VersusDbContext : DbContext
{
    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();

    public VersusDbContext(DbContextOptions<VersusDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ...
    }
}