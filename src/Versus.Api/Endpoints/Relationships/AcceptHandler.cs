using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Shared.Relationships;

namespace Versus.Api.Endpoints.Relationships;

public record AcceptHandlerParameters
{
    public ClaimsPrincipal User { get; init; } = default!;
    public AcceptRequest Request { get; init; } = default!;
    public VersusDbContext DbContext { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out ClaimsPrincipal user,
        out AcceptRequest request,
        out VersusDbContext dbContext,
        out CancellationToken cancellationToken)
    {
        user = User;
        request = Request;
        dbContext = DbContext;
        cancellationToken = CancellationToken;
    }
}

public static class AcceptHandler
{
    public static async Task<Results<Ok, ProblemHttpResult, UnauthorizedHttpResult>> HandleAsync
        ([AsParameters] AcceptHandlerParameters parameters)
    {
        var (user, request, dbContext, cancellationToken) = parameters;

        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var relationship = await dbContext.UserRelationships
            .Where(x => x.Id == request.Id
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

        relationship.Status = UserRelationshipStatus.Accepted;
        relationship.Timestamp = TimeProvider.System.GetUtcNow().UtcDateTime;
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}