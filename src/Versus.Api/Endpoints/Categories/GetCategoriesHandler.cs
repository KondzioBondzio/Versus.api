using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Categories;
using Versus.Shared.Common;

namespace Versus.Api.Endpoints.Categories;

public class GetCategoriesHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapGet("/", HandleAsync)
        .WithRequestValidation<GetCategoriesRequest>()
        .Produces<Ok<PagedResponse<GetCategoriesResponse>>>()
        .Produces<UnauthorizedHttpResult>();

    public static async Task<IResult> HandleAsync(
        [AsParameters] GetCategoriesRequest request,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var data = await dbContext.Categories
            .AsNoTracking()
            .Skip((request.PageNumber - 1) * request.PageSize ?? 0)
            .Take(request.PageSize ?? 10)
            .Select(ProjectTo)
            .ToListAsync(cancellationToken);

        var result = new PagedResponse<GetCategoriesResponse>(0, data.Count, data.Count, data);
        return TypedResults.Ok(result);
    }

    private static Expression<Func<Category, GetCategoriesResponse>> ProjectTo => src => new GetCategoriesResponse
    {
        Id = src.Id,
        Name = src.Name,
        Description = src.Description,
        Image = src.Image
    };
}