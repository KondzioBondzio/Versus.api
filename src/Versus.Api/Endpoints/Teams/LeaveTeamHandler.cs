using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Extensions;

namespace Versus.Api.Endpoints.Teams;

public class LeaveTeamHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/{id:guid}/leave", HandleAsync)
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

        var teamUser = await dbContext.TeamUsers
            .SingleOrDefaultAsync(x => x.TeamId == id && x.UserId == userId, cancellationToken);
        if (teamUser == null)
        {
            return TypedResults.NotFound();
        }

        dbContext.TeamUsers.Remove(teamUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}