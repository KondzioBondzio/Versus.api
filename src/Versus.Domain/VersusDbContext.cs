using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Versus.Domain.Entities;

namespace Versus.Domain;

public class VersusDbContext : IdentityDbContext<User, Role, int>
{
    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();

    public VersusDbContext(DbContextOptions<VersusDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => base.OnModelCreating(modelBuilder);
}
