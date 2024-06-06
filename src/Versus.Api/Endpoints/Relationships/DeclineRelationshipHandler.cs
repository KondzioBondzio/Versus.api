using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Relationships;

namespace Versus.Api.Endpoints.Relationships;

public class DeclineRelationshipHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/decline", HandleAsync)
        .WithRequestValidation<BlockRelationshipRequest>();

    public static async Task<Results<Ok, ProblemHttpResult, UnauthorizedHttpResult>> HandleAsync(
        DeclineRelationshipRequest relationshipRequest,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var relationship = await dbContext.UserRelationships
            .Where(x => x.Id == relationshipRequest.Id
                        && x.Status == UserRelationshipStatus.Pending
                        && x.RelatedUserId == userId)
            .SingleOrDefaultAsync(cancellationToken);
        if (relationship == null)
        {
            return TypedResults.Problem(
                title: "Relationship not found",
                detail: "RelationshipNotFound",
                statusCode: StatusCodes.Status400BadRequest);
        }

        relationship.Status = UserRelationshipStatus.Declined;
        relationship.Timestamp = TimeProvider.System.GetUtcNow().UtcDateTime;
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}