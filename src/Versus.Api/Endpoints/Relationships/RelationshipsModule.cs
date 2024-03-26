using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;

namespace Versus.Api.Endpoints.Relationships;

[EndpointGroupName("Relationships")]
public class RelationshipsModule : IModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        services.AddTransient(sp =>
            sp.GetRequiredService<VersusDbContext>().Set<UserRelationship>().AsNoTracking());

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder builder)
    {
        var routeGroup = builder
            .MapGroup(string.Empty)
            .RequireAuthorization();

        routeGroup.MapGet("/", RelationshipsHandler.HandleAsync);
        routeGroup.MapPost("/friend", FriendHandler.HandleAsync);
        routeGroup.MapPost("/accept", AcceptHandler.HandleAsync);
        routeGroup.MapPost("/decline", DeclineHandler.HandleAsync);
        routeGroup.MapPost("/unfriend", UnfriendHandler.HandleAsync);
        routeGroup.MapPost("/block", BlockHandler.HandleAsync);
        routeGroup.MapPost("/unblock", UnblockHandler.HandleAsync);

        return routeGroup;
    }
}