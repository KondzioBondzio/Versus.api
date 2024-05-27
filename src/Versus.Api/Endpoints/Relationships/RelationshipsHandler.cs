using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Common;
using Versus.Shared.Relationships;

namespace Versus.Api.Endpoints.Relationships;

public record RelationshipsParameters
{
    public ClaimsPrincipal ClaimsPrincipal { get; init; } = default!;
    public VersusDbContext DbContext { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out ClaimsPrincipal claimsPrincipal,
        out VersusDbContext dbContext,
        out CancellationToken cancellationToken)
    {
        claimsPrincipal = ClaimsPrincipal;
        dbContext = DbContext;
        cancellationToken = CancellationToken;
    }
}

public class RelationshipsHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapGet("/", HandleAsync);

    public static async Task<Results<Ok<PagedResponse<RelationshipResponse>>, UnauthorizedHttpResult>> HandleAsync
        ([AsParameters] RelationshipsParameters parameters)
    {
        var (claimsPrincipal, dbContext, cancellationToken) = parameters;

        var userId = claimsPrincipal.GetUserId();

        var relationships = await dbContext.UserRelationships
            .AsNoTracking()
            .Where(x => x.UserId == userId || x.RelatedUserId == userId)
            // .Skip((request.PageNumber - 1) * request.PageSize ?? 0)
            // .Take(request.PageSize ?? 10)
            .Select(Map(userId))
            .ToListAsync(cancellationToken);

        var result = new PagedResponse<RelationshipResponse>(0, relationships.Count, relationships.Count, relationships);
        return TypedResults.Ok(result);
    }

    private static Expression<Func<UserRelationship, RelationshipResponse>> Map(Guid userId) => src => new RelationshipResponse
    {
        Id = src.Id, 
        UserId = src.UserId == userId ? src.RelatedUserId : src.UserId, 
        Direction = src.UserId == userId
            ? RelationshipResponse.RelationshipDirection.Outgoing
            : RelationshipResponse.RelationshipDirection.Incoming,
        Status = (int)src.Status, 
        Type = (int)src.Type
    };
}