﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Versus.Api.Data;
using Versus.Shared.Auth;

namespace Versus.Api.Tests;

internal class WebAppFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
        keepAliveConnection.Open();

        _ = builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<VersusDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContextPool<VersusDbContext>(options =>
                options.UseSqlite(keepAliveConnection));


            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<VersusDbContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            // migrate initial database state
            new TestDatabaseSeeder(scope.ServiceProvider).SeedDatabase(dbContext).GetAwaiter().GetResult();
        });
    }

    public async Task<HttpClient> CreateAuthenticatedClient()
    {
        var client = CreateClient();
        var authRequest = new LoginRequest
        {
            Login = "User1@test.com", Password = "Qwerty1!"
        };
        
        var response = await client.PostAsJsonAsync("/api/auth/login", authRequest);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result!.AccessToken);
        
        return client;
    }
}