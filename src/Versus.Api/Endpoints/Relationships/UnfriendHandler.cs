using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Shared.Relationships;

namespace Versus.Api.Endpoints.Relationships;

public record UnfriendParameters
{
    public ClaimsPrincipal User { get; init; } = default!;
    public UnfriendRequest Request { get; init; } = default!;
    public VersusDbContext DbContext { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out ClaimsPrincipal user,
        out UnfriendRequest request,
        out VersusDbContext dbContext,
        out CancellationToken cancellationToken)
    {
        user = User;
        request = Request;
        dbContext = DbContext;
        cancellationToken = CancellationToken;
    }
}

public static class UnfriendHandler
{
    public static async Task<Results<Ok, ProblemHttpResult, UnauthorizedHttpResult>> HandleAsync
        ([AsParameters] UnfriendParameters parameters)
    {
        var (user, request, dbContext, cancellationToken) = parameters;

        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var relationship = await dbContext.UserRelationships
            .AsNoTracking()
            .Where(x => x.Type == UserRelationshipType.Friend
                        && (x.Status == UserRelationshipStatus.Accepted || x.Status == UserRelationshipStatus.Pending)
                        && (x.UserId == userId || x.RelatedUserId == userId)
                        && (x.UserId == request.Id || x.RelatedUserId == request.Id))
            .SingleOrDefaultAsync(cancellationToken);
        if (relationship == null)
        {
            return TypedResults.Problem(
                title: "Users are not friends",
                detail: "UsersNotFriends",
                statusCode: StatusCodes.Status400BadRequest);
        }

        dbContext.UserRelationships.Remove(relationship);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}