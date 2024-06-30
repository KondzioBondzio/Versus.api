using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Versus.Api.Data;
using Versus.Api.Entities;

namespace Versus.Api.Tests.DataSources;

public class VersusTestDatabaseSeeder(IServiceProvider serviceProvider)
{
    private readonly VersusDbContext _dbContext = serviceProvider.GetRequiredService<VersusDbContext>();

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await _dbContext.Database.EnsureCreatedAsync(cancellationToken);

        await SeedPermissions(cancellationToken);
        await SeedRoles(cancellationToken);
        await SeedUsers(cancellationToken);
        await SeedCategories(cancellationToken);
        await SeedRooms(cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedPermissions(CancellationToken cancellationToken)
    {
        // ...

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedRoles(CancellationToken cancellationToken)
    {
        // ...

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedUsers(CancellationToken cancellationToken)
    {
        foreach (var value in Enumerable.Range(1, 10))
        {
            var user = CreateDemoUserFromLogin(Constants.GetTemplatedLogin(value));
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            await userManager.CreateAsync(user, Constants.UserPassword);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedCategories(CancellationToken cancellationToken)
    {
        var categories = Enumerable.Range(1, 10)
            .Select(x => new Category
            {
                Name = $"Category {x}"
            })
            .ToList();

        await _dbContext.Categories.AddRangeAsync(categories, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedRooms(CancellationToken cancellationToken)
    {
        var userId = await _dbContext.Users.Select(x => x.Id).FirstAsync(cancellationToken);
        var categoryId = await _dbContext.Categories.Select(x => x.Id).FirstAsync(cancellationToken);

        var rooms = Enumerable.Range(1, 10)
            .Select(x => new Room
            {
                Name = $"Room {x}",
                Description = string.Empty,
                PlayDate = DateTime.UtcNow.AddDays(7),
                Status = RoomStatus.Unconfirmed,
                CategoryId = categoryId,
                HostId = userId,
                TeamPlayerLimit = 5,
                Teams = Enumerable.Range(1, 2).Select(y => new Team
                {
                    Name = $"Team {y}"
                }).ToList()
            })
            .ToList();

        await _dbContext.Rooms.AddRangeAsync(rooms, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static User CreateDemoUserFromLogin(string login)
    {
        return new User
        {
            UserName = login,
            Email = login,
            City = "Warsaw",
            Gender = UserGender.Female,
            LanguageCode = "pl",
            YearOfBirth = 2000
        };
    }
}