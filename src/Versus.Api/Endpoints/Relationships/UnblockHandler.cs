using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Shared.Relationships;

namespace Versus.Api.Endpoints.Relationships;

public record UnblockParameters
{
    public ClaimsPrincipal User { get; init; } = default!;
    public UnblockRequest Request { get; init; } = default!;
    public VersusDbContext DbContext { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out ClaimsPrincipal user,
        out UnblockRequest request,
        out VersusDbContext dbContext,
        out CancellationToken cancellationToken)
    {
        user = User;
        request = Request;
        dbContext = DbContext;
        cancellationToken = CancellationToken;
    }
}

public static class UnblockHandler
{
    public static async Task<Results<Ok, ProblemHttpResult, UnauthorizedHttpResult>> HandleAsync
        ([AsParameters] UnblockParameters parameters)
    {
        var (user, request, dbContext, cancellationToken) = parameters;

        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var relationship = await dbContext.UserRelationships
            .AsNoTracking()
            .Where(x => x.Type == UserRelationshipType.Block
                        && x.Status == UserRelationshipStatus.Accepted
                        && (x.UserId == userId || x.RelatedUserId == userId)
                        && (x.UserId == request.Id || x.RelatedUserId == request.Id))
            .SingleOrDefaultAsync(cancellationToken);
        if (relationship == null)
        {
            return TypedResults.Problem(
                title: "User is not blocked",
                detail: "UserNotBlocked",
                statusCode: StatusCodes.Status400BadRequest);
        }

        dbContext.UserRelationships.Remove(relationship);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}