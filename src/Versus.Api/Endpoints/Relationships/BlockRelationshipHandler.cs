using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Relationships;

namespace Versus.Api.Endpoints.Relationships;

public class BlockRelationshipHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/block", HandleAsync)
        .WithRequestValidation<BlockRelationshipRequest>()
        .Produces<Ok>()
        .Produces<NotFound>()
        .Produces<UnauthorizedHttpResult>();

    public static async Task<IResult> HandleAsync(
        BlockRelationshipRequest relationshipRequest,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        bool receiver = await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == relationshipRequest.Id)
            .AnyAsync(cancellationToken);
        if (!receiver)
        {
            return TypedResults.NotFound();
        }

        var relationship = await dbContext.UserRelationships
            .AsNoTracking()
            .Where(x => (x.UserId == userId || x.RelatedUserId == userId)
                        && (x.UserId == relationshipRequest.Id || x.RelatedUserId == relationshipRequest.Id))
            .SingleOrDefaultAsync(cancellationToken);
        if (relationship is { Type: UserRelationshipType.Block, Status: UserRelationshipStatus.Accepted })
        {
            return TypedResults.Ok();
        }

        if (relationship != null)
        {
            dbContext.UserRelationships.Remove(relationship);
        }

        relationship = new UserRelationship
        {
            UserId = userId,
            RelatedUserId = relationshipRequest.Id,
            Type = UserRelationshipType.Block,
            Status = UserRelationshipStatus.Accepted,
            Timestamp = TimeProvider.System.GetUtcNow().UtcDateTime
        };
        await dbContext.UserRelationships.AddAsync(relationship, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}