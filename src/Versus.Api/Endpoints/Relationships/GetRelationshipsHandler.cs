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

public class GetRelationshipsHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapGet("/", HandleAsync)
        .WithRequestValidation<GetRelationshipsRequest>()
        .Produces<Ok<PagedResponse<GetRelationshipsResponse>>>()
        .Produces<UnauthorizedHttpResult>();

    public static async Task<IResult> HandleAsync(
        [AsParameters] GetRelationshipsRequest request,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var data = await dbContext.UserRelationships
            .AsNoTracking()
            .Where(x => x.UserId == userId || x.RelatedUserId == userId)
            .Skip((request.PageNumber - 1) * request.PageSize ?? 0)
            .Take(request.PageSize ?? 10)
            .Select(ProjectTo(userId))
            .ToListAsync(cancellationToken);

        var result = new PagedResponse<GetRelationshipsResponse>(0, data.Count, data.Count, data);
        return TypedResults.Ok(result);
    }

    private static Expression<Func<UserRelationship, GetRelationshipsResponse>> ProjectTo(Guid userId) => src =>
        new GetRelationshipsResponse
        {
            Id = src.Id,
            UserId = src.UserId == userId ? src.RelatedUserId : src.UserId,
            Direction = src.UserId == userId
                ? GetRelationshipsResponse.RelationshipDirection.Outgoing
                : GetRelationshipsResponse.RelationshipDirection.Incoming,
            Status = (int)src.Status,
            Type = (int)src.Type
        };
}