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
        await SeedPermissions(dbContext.Permissions, cancellationToken);
        await SeedRoles(dbContext.Roles, cancellationToken);
        await SeedUsers(dbContext.Users, cancellationToken);
        await SeedCategories(dbContext.Categories, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedPermissions(DbSet<Permission> set, CancellationToken cancellationToken = default)
    {
    }

    private async Task SeedRoles(DbSet<Role> set, CancellationToken cancellationToken = default)
    {
    }

    private async Task SeedUsers(DbSet<User> set, CancellationToken cancellationToken = default)
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
    }

    private async Task SeedCategories(DbSet<Category> set, CancellationToken cancellationToken = default)
    {
        var categories = Enumerable.Range(0, 100)
            .Select(x => new Category
            {
                Name = $"Category {x}"
            })
            .ToList();

        await set.AddRangeAsync(categories, cancellationToken);
    }
}