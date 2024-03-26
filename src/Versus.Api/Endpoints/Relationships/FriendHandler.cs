using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Shared.Relationships;

namespace Versus.Api.Endpoints.Relationships;

public record FriendParameters
{
    public ClaimsPrincipal User { get; init; } = default!;
    public FriendRequest Request { get; init; } = default!;
    public VersusDbContext DbContext { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out ClaimsPrincipal user,
        out FriendRequest request,
        out VersusDbContext dbContext,
        out CancellationToken cancellationToken)
    {
        user = User;
        request = Request;
        dbContext = DbContext;
        cancellationToken = CancellationToken;
    }
}

public static class FriendHandler
{
    public static async Task<Results<Ok, ProblemHttpResult, UnauthorizedHttpResult>> HandleAsync
        ([AsParameters] FriendParameters parameters)
    {
        var (user, request, dbContext, cancellationToken) = parameters;

        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return TypedResults.Unauthorized();
        }

        bool receiver = await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .AnyAsync(cancellationToken);
        if (!receiver)
        {
            return TypedResults.Problem(
                title: "User not found",
                detail: "UserNotFound",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var relationship = await dbContext.UserRelationships
            .Where(x => (x.UserId == userId || x.RelatedUserId == userId)
                        && (x.UserId == request.Id || x.RelatedUserId == request.Id))
            .SingleOrDefaultAsync(cancellationToken);
        if (relationship is { Type: UserRelationshipType.Friend, Status: UserRelationshipStatus.Pending })
        {
            return TypedResults.Ok();
        }

        if (relationship is { Type: UserRelationshipType.Friend, Status: UserRelationshipStatus.Accepted })
        {
            return TypedResults.Problem(
                title: "User is already a friend",
                detail: "UsersAlreadyFriends",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (relationship is { Type: UserRelationshipType.Block, Status: UserRelationshipStatus.Accepted })
        {
            return TypedResults.Problem(
                title: "Users is blocking / blocked by receiver",
                detail: "UserBlock",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (relationship != null)
        {
            return TypedResults.Problem(
                title: "Users already in relationship",
                detail: "UsersAlreadyInRelationship",
                statusCode: StatusCodes.Status400BadRequest);
        }

        relationship = new UserRelationship
        {
            UserId = userId,
            RelatedUserId = request.Id,
            Type = UserRelationshipType.Friend,
            Status = UserRelationshipStatus.Pending,
            Timestamp = TimeProvider.System.GetUtcNow().UtcDateTime
        };
        await dbContext.UserRelationships.AddAsync(relationship, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}