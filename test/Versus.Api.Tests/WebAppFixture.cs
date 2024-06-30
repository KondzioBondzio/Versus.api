using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Services.Auth;
using Versus.Api.Tests.DataSources;

namespace Versus.Api.Tests;

internal class WebAppFixture : WebApplicationFactory<Program>
{
    public VersusDbContext DbContext => Services.CreateScope().ServiceProvider.GetRequiredService<VersusDbContext>();

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

            services.AddDbContextPool<VersusDbContext>(options => options.UseSqlite(keepAliveConnection));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            new VersusTestDatabaseSeeder(scope.ServiceProvider)
                .SeedAsync(CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        });
    }

    public HttpClient CreateAuthenticatedClient(User user)
    {
        var tokenService = Services.CreateScope().ServiceProvider.GetRequiredService<ITokenService>();
        var accessToken = tokenService.GenerateAccessToken(user);

        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return client;
    }
}