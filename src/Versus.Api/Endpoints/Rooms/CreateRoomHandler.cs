using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Rooms;

namespace Versus.Api.Endpoints.Rooms;

public class CreateRoomHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/", HandleAsync)
        .WithRequestValidation<CreateRoomRequest>();

    public static async Task<Results<Created, ProblemHttpResult, UnauthorizedHttpResult>> HandleAsync(
        CreateRoomRequest request,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var room = new Room
        {
            Name = request.Name,
            Password = request.Password,
            CategoryId = request.CategoryId,
            HostId = userId,
            TeamPlayerLimit = request.TeamPlayerLimit
        };

        foreach (var i in Enumerable.Range(0, request.TeamCount))
        {
            var team = new Team
            {
                Name = $"Team {i}",
            };
            room.Teams.Add(team);
        }

        await dbContext.Rooms.AddAsync(room, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Created();
    }
}