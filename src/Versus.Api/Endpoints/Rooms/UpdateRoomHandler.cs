using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Extensions;
using Versus.Shared.Rooms;

namespace Versus.Api.Endpoints.Rooms;

public class UpdateRoomHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPatch("/{id:guid}", HandleAsync)
        .WithRequestValidation<CreateRoomRequest>()
        .Produces<NoContent>()
        .Produces<ValidationProblem>()
        .Produces<UnauthorizedHttpResult>()
        .Produces<ForbidHttpResult>();

    public static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateRoomRequest request,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var isHost = await dbContext.Rooms
            .AsNoTracking()
            .AnyAsync(x => x.Id == id && x.HostId == userId, cancellationToken);
        if (!isHost)
        {
            return TypedResults.Forbid();
        }
        
        // ...

        return TypedResults.NoContent();
    }
}