using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Extensions;
using Versus.Shared.Rooms;

namespace Versus.Api.Endpoints.Rooms;

public class DeleteRoomHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapDelete("/{id:guid}", HandleAsync)
        .WithRequestValidation<GetRoomsRequest>()
        .Produces<NoContent>()
        .Produces<NotFound>()
        .Produces<UnauthorizedHttpResult>()
        .Produces<ForbidHttpResult>();

    public static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var room = await dbContext.Rooms
            .Include(x => x.Users)
            .Include(x => x.Teams)
            .ThenInclude(x => x.Users)
            .SingleOrDefaultAsync(cancellationToken);
        if (room == null)
        {
            return TypedResults.NotFound();
        }

        if (room.HostId != userId)
        {
            return TypedResults.Forbid();
        }

        // TODO: Refactor
        dbContext.RoomChatMessages.RemoveRange(room.ChatMessages);
        dbContext.TeamUsers.RemoveRange(room.Teams.SelectMany(x => x.Users));
        dbContext.Teams.RemoveRange(room.Teams);
        dbContext.RoomUsers.RemoveRange(room.Users);
        dbContext.Rooms.Remove(room);

        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}