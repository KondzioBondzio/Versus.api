using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Extensions;

namespace Versus.Api.Endpoints.Rooms;

public class LeaveRoomHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/{id:guid}", HandleAsync)
        .Produces<NoContent>()
        .Produces<NotFound>()
        .Produces<UnauthorizedHttpResult>();

    public static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var roomUser = await dbContext.RoomUsers
            .SingleOrDefaultAsync(x => x.RoomId == id && x.UserId == userId, cancellationToken);
        if (roomUser != null)
        {
            dbContext.RoomUsers.Remove(roomUser);
            await dbContext.SaveChangesAsync(cancellationToken);
            return TypedResults.NoContent();
        }

        var roomTeamUser = await dbContext.TeamUsers
            .Include(x => x.Team)
            .SingleOrDefaultAsync(x => x.Team.RoomId == id && x.UserId == userId, cancellationToken);
        if (roomTeamUser != null)
        {
            dbContext.TeamUsers.Remove(roomTeamUser);
            await dbContext.SaveChangesAsync(cancellationToken);
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound();
    }
}