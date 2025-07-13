using FluentValidation;
using Serilog;
using Versus.Api.Exceptions;
using Versus.Api.Extensions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Versus.Api.Tests")]

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Host.AddLogging(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIdentity(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddVersusFeatureManagement();
builder.Services.AddVersusServices(builder.Configuration);

builder.Services.AddExceptionHandler<ApiExceptionHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(Environments.Development, policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
    options.AddPolicy(Environments.Production, policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

WebApplication app = builder.Build();

app.UseSerilogRequestLogging(options => { options.IncludeQueryInRequestPath = true; });

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// order of items below does matter!
// see: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#middleware-order
if (app.Environment.IsProduction())
{
    app.UseExceptionHandler(_ => { });
    app.UseHsts(); // do not ignore and allow users to bypass ssl certificate errors
    app.UseHttpsRedirection(); // redirects all http requests to https
}

app.UseCors(app.Environment.EnvironmentName);
app.UseAuthentication();
app.UseAuthorization();

// custom middleware

app.MapEndpoints();

// using IServiceScope scope = app.Services.CreateScope();
// var migrator = scope.ServiceProvider.GetRequiredService<VersusMigrator>();
// await migrator.MigrateAsync();

await app.RunAsync();