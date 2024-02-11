using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Versus.Domain;

namespace Versus.Host.Migrations;

public sealed class VersusMigrator
{
    private readonly ILogger<VersusMigrator> _logger;
    private readonly VersusDbContext _dbContext;

    public VersusMigrator(ILogger<VersusMigrator> logger, VersusDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task MigrateAsync()
    {
        IEnumerable<string> pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
        if (!pendingMigrations.Any())
        {
            _logger.LogInformation("No pending migrations found. Database schema is up to date");
            return;
        }

        _logger.LogInformation("Pending migrations found. Migrating..");
        await _dbContext.Database.MigrateAsync();
        _logger.LogInformation("Migration complete.");
    }
}
