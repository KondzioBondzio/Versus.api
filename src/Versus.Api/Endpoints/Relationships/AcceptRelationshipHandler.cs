using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Relationships;

namespace Versus.Api.Endpoints.Relationships;

public class AcceptRelationshipHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/accept", HandleAsync)
        .WithRequestValidation<AcceptRelationshipRequest>()
        .Produces<Ok>()
        .Produces<NotFound>()
        .Produces<UnauthorizedHttpResult>();

    public static async Task<IResult> HandleAsync(
        AcceptRelationshipRequest request,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var relationship = await dbContext.UserRelationships
            .Where(x => x.Id == request.Id
                        && x.Status == UserRelationshipStatus.Pending
                        && x.RelatedUserId == userId)
            .SingleOrDefaultAsync(cancellationToken);
        if (relationship == null)
        {
            return TypedResults.NotFound();
        }

        relationship.Status = UserRelationshipStatus.Accepted;
        relationship.Timestamp = TimeProvider.System.GetUtcNow().UtcDateTime;
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}