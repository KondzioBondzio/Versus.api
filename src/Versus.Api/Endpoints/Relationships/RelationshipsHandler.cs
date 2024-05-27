using System.Security.Claims;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Extensions;
using Versus.Shared.Common;
using Versus.Shared.Relationships;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Versus.Api.Endpoints.Relationships;

public record RelationshipsParameters
{
    [FromQuery]
    public RelationshipsRequest Request { get; init; } = default!;
    public ClaimsPrincipal ClaimsPrincipal { get; init; } = default!;
    public VersusDbContext DbContext { get; init; } = default!;
    public IConfigurationProvider ConfigurationProvider { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out RelationshipsRequest request, 
        out ClaimsPrincipal claimsPrincipal,
        out VersusDbContext dbContext,
        out IConfigurationProvider configurationProvider,
        out CancellationToken cancellationToken)
    {
        request = Request;
        claimsPrincipal = ClaimsPrincipal;
        dbContext = DbContext;
        configurationProvider = ConfigurationProvider;
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
        var (request, claimsPrincipal, dbContext, mapperConfig, cancellationToken) = parameters;

        var userId = claimsPrincipal.GetUserId();

        var relationships = await dbContext.UserRelationships
            .AsNoTracking()
            .Where(x => x.UserId == userId || x.RelatedUserId == userId)
            .Skip((request.PageNumber - 1) * request.PageSize ?? 0)
            .Take(request.PageSize ?? 10)
            .ProjectTo<RelationshipResponse>(mapperConfig)
            .ToListAsync(cancellationToken);

        var result = new PagedResponse<RelationshipResponse>(0, relationships.Count, relationships.Count, relationships);
        return TypedResults.Ok(result);
    }
}