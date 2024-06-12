using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Rooms;

namespace Versus.Api.Endpoints.Rooms;

public class JoinRoomHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/{id:guid}/join", HandleAsync)
        .WithRequestValidation<JoinRoomRequest>();

    public static async Task<Results<Ok, NotFound, ValidationProblem, UnauthorizedHttpResult>> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] JoinRoomRequest request,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var room = await dbContext.Rooms
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (room == null)
        {
            return TypedResults.NotFound();
        }

        // TODO: Refactor
        if (!string.IsNullOrEmpty(room.Password) && room.Password != request.Password)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                {
                    nameof(request.Password), ["Invalid password"]
                }
            });
        }
        
        // TODO: Refactor
        var isRoomOwner = room.HostId == userId;
        if (isRoomOwner)
        {
            return TypedResults.Ok();
        }
        
        // TODO: Refactor
        var hasAlreadyJoinedRoom = await dbContext.RoomUsers
            .AsNoTracking()
            .AnyAsync(x => x.RoomId == room.Id && x.UserId == userId, cancellationToken);
        if (hasAlreadyJoinedRoom)
        {
            return TypedResults.Ok();
        }

        room.Users.Add(new RoomUser
        {
            RoomId = room.Id, UserId = userId
        });
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}