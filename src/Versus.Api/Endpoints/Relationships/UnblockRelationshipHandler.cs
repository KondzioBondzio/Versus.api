using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Relationships;

namespace Versus.Api.Endpoints.Relationships;

public class UnblockRelationshipHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/unblock", HandleAsync)
        .WithRequestValidation<UnblockFriendshipRequest>()
        .Produces<Ok>()
        .Produces<NotFound>()
        .Produces<UnauthorizedHttpResult>();

    public static async Task<IResult> HandleAsync(
        UnblockFriendshipRequest friendshipRequest,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var relationship = await dbContext.UserRelationships
            .AsNoTracking()
            .Where(x => x.Type == UserRelationshipType.Block
                        && x.Status == UserRelationshipStatus.Accepted
                        && (x.UserId == userId || x.RelatedUserId == userId)
                        && (x.UserId == friendshipRequest.Id || x.RelatedUserId == friendshipRequest.Id))
            .SingleOrDefaultAsync(cancellationToken);
        if (relationship == null)
        {
            return TypedResults.NotFound();
        }

        dbContext.UserRelationships.Remove(relationship);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}