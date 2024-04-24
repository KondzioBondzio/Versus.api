using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Relationships;

namespace Versus.Api.Endpoints.Relationships;

public record BlockParameters
{
    public ClaimsPrincipal ClaimsPrincipal { get; init; } = default!;
    public BlockRequest Request { get; init; } = default!;
    public VersusDbContext DbContext { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out ClaimsPrincipal claimsPrincipal,
        out BlockRequest request,
        out VersusDbContext dbContext,
        out CancellationToken cancellationToken)
    {
        claimsPrincipal = ClaimsPrincipal;
        request = Request;
        dbContext = DbContext;
        cancellationToken = CancellationToken;
    }
}

public class BlockHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/block", HandleAsync);

    public static async Task<Results<Ok, ProblemHttpResult, UnauthorizedHttpResult>> HandleAsync
        ([AsParameters] BlockParameters parameters)
    {
        var (claimsPrincipal, request, dbContext, cancellationToken) = parameters;

        var userId = claimsPrincipal.GetUserId();

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