﻿using Serilog;
using Versus.Api.Entities;
using Versus.Api.Exceptions;
using Versus.Api.Extensions;
using Versus.Api.Migrations;

Log.Logger = new LoggerConfiguration()
    .CreateBootstrapLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Host.AddLogging(builder.Configuration);

builder.Services.AddSwagger();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIdentity(builder.Configuration);
builder.Services.AddVersusServices(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddExceptionHandler<ApiExceptionHandler>();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging(options => { options.IncludeQueryInRequestPath = true; });

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(_ => { });

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("api/v1/auth")
    .MapIdentityApi<User>();

app.MapControllers();

try
{
    using IServiceScope scope = app.Services.CreateScope();
    IServiceProvider services = scope.ServiceProvider;
    VersusMigrator migrator = services.GetRequiredService<VersusMigrator>();
    await migrator.MigrateAsync();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Startup failed");
}
