using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Shared.Relationships;

namespace Versus.Api.Endpoints.Relationships;

public record BlockParameters
{
    public ClaimsPrincipal User { get; init; } = default!;
    public BlockRequest Request { get; init; } = default!;
    public VersusDbContext DbContext { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out ClaimsPrincipal user,
        out BlockRequest request,
        out VersusDbContext dbContext,
        out CancellationToken cancellationToken)
    {
        user = User;
        request = Request;
        dbContext = DbContext;
        cancellationToken = CancellationToken;
    }
}

public static class BlockHandler
{
    public static async Task<Results<Ok, ProblemHttpResult, UnauthorizedHttpResult>> HandleAsync
        ([AsParameters] BlockParameters parameters)
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
            .AsNoTracking()
            .Where(x => (x.UserId == userId || x.RelatedUserId == userId)
                        && (x.UserId == request.Id || x.RelatedUserId == request.Id))
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
            RelatedUserId = request.Id,
            Type = UserRelationshipType.Block,
            Status = UserRelationshipStatus.Accepted,
            Timestamp = TimeProvider.System.GetUtcNow().UtcDateTime
        };
        await dbContext.UserRelationships.AddAsync(relationship, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}