using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Relationships;

namespace Versus.Api.Endpoints.Relationships;

public record UnblockParameters
{
    public ClaimsPrincipal ClaimsPrincipal { get; init; } = default!;
    public UnblockRequest Request { get; init; } = default!;
    public VersusDbContext DbContext { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out ClaimsPrincipal claimsPrincipal,
        out UnblockRequest request,
        out VersusDbContext dbContext,
        out CancellationToken cancellationToken)
    {
        claimsPrincipal = ClaimsPrincipal;
        request = Request;
        dbContext = DbContext;
        cancellationToken = CancellationToken;
    }
}

public class UnblockHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/unblock", HandleAsync);

    public static async Task<Results<Ok, ProblemHttpResult, UnauthorizedHttpResult>> HandleAsync
        ([AsParameters] UnblockParameters parameters)
    {
        var (claimsPrincipal, request, dbContext, cancellationToken) = parameters;

        var userId = claimsPrincipal.GetUserId();

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