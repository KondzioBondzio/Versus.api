using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Relationships;

namespace Versus.Api.Endpoints.Relationships;

public record AcceptHandlerParameters
{
    public ClaimsPrincipal ClaimsPrincipal { get; init; } = default!;
    public AcceptRequest Request { get; init; } = default!;
    public VersusDbContext DbContext { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out ClaimsPrincipal claimsPrincipal,
        out AcceptRequest request,
        out VersusDbContext dbContext,
        out CancellationToken cancellationToken)
    {
        claimsPrincipal = ClaimsPrincipal;
        request = Request;
        dbContext = DbContext;
        cancellationToken = CancellationToken;
    }
}

public class AcceptHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/accept", HandleAsync);
    
    public static async Task<Results<Ok, ProblemHttpResult, UnauthorizedHttpResult>> HandleAsync
        ([AsParameters] AcceptHandlerParameters parameters)
    {
        var (claimsPrincipal, request, dbContext, cancellationToken) = parameters;

        var userId = claimsPrincipal.GetUserId();

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