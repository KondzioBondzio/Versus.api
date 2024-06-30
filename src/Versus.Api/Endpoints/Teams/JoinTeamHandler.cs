using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;

namespace Versus.Api.Endpoints.Teams;

public class JoinTeamHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/{id:guid}/join", HandleAsync)
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

        var exists = await dbContext.Teams
            .AsNoTracking()
            .AnyAsync(x => x.Id == id, cancellationToken);
        if (!exists)
        {
            return TypedResults.NotFound();
        }

        var teamUser = new TeamUser
        {
            TeamId = id,
            UserId = userId
        };
        await dbContext.TeamUsers.AddAsync(teamUser, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}