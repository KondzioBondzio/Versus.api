using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Shared.Relationships;

namespace Versus.Api.Endpoints.Relationships;

public record RelationshipsParameters
{
    public ClaimsPrincipal User { get; init; } = default!;
    public VersusDbContext DbContext { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out ClaimsPrincipal user,
        out VersusDbContext dbContext,
        out CancellationToken cancellationToken)
    {
        user = User;
        dbContext = DbContext;
        cancellationToken = CancellationToken;
    }
}

public static class RelationshipsHandler
{
    public static async Task<Results<Ok<IEnumerable<RelationshipDto>>, UnauthorizedHttpResult>> HandleAsync
        ([AsParameters] RelationshipsParameters parameters)
    {
        var (user, dbContext, cancellationToken) = parameters;

        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var relationships = await dbContext.UserRelationships
            .AsNoTracking()
            .Where(x => x.UserId == userId || x.RelatedUserId == userId)
            .Select(x => new RelationshipDto
            {
                Id = x.Id,
                UserId = x.UserId == userId ? x.RelatedUserId : x.UserId,
                Direction = x.UserId == userId
                    ? RelationshipDto.RelationshipDirection.Outgoing
                    : RelationshipDto.RelationshipDirection.Incoming,
                Status = (int)x.Status,
                Type = (int)x.Type,
            })
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(relationships.AsEnumerable());
    }
}