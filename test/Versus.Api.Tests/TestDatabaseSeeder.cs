using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Versus.Api.Data;
using Versus.Api.Entities;

namespace Versus.Api.Tests;

public class TestDatabaseSeeder
{
    private readonly IServiceProvider _serviceProvider;

    public TestDatabaseSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedDatabase(VersusDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await SeedPermissions(dbContext, cancellationToken);
        await SeedRoles(dbContext, cancellationToken);
        await SeedUsers(dbContext, cancellationToken);
        await SeedCategories(dbContext, cancellationToken);
        await SeedRooms(dbContext, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedPermissions(VersusDbContext dbContext, CancellationToken cancellationToken = default)
    {
        // ...
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedRoles(VersusDbContext dbContext, CancellationToken cancellationToken = default)
    {
        // ...
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedUsers(VersusDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await CreateUser("User1@test.com", "Qwerty1!");
        await CreateUser("User2@test.com", "Qwerty1!");

        Task CreateUser(string login, string password)
        {
            var u = new User
            {
                UserName = login,
                Email = login,
                City = "Warsaw",
                Gender = UserGender.Female,
                LanguageCode = "pl",
                YearOfBirth = 2000
            };
            var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
            return userManager.CreateAsync(u, password);
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedCategories(VersusDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var categories = Enumerable.Range(0, 100)
            .Select(x => new Category
            {
                Name = $"Category {x}"
            })
            .ToList();

        await dbContext.Categories.AddRangeAsync(categories, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    private async Task SeedRooms(VersusDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var userId = await dbContext.Users.Select(x => x.Id).FirstAsync(cancellationToken);
        var categoryId = await dbContext.Categories.Select(x => x.Id).FirstAsync(cancellationToken);
        
        var rooms = Enumerable.Range(0, 100)
            .Select(x => new Room
            {
                Name = $"Room {x}",
                CategoryId = categoryId,
                HostId = userId,
                TeamPlayerLimit = 5,
                Teams = Enumerable.Range(0, 2).Select(y => new Team { Name = $"Team {y}" }).ToList()
            })
            .ToList();

        await dbContext.Rooms.AddRangeAsync(rooms, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}