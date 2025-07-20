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

public class GetCategoryHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapGet("{id:Guid?}", HandleAsync)
        .WithRequestValidation<GetCategoriesRequest>()
        .Produces<Ok<PagedResponse<GetCategoryResponse>>>()
        .Produces<UnauthorizedHttpResult>();

    public static async Task<IResult> HandleAsync(
        Guid? id,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (!id.HasValue)
        {
            return TypedResults.BadRequest();
        }

        var category = await dbContext.Categories
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(ProjectTo)
            .SingleOrDefaultAsync(cancellationToken);
        if (category is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(category);
    }

    private static Expression<Func<Category, GetCategoryResponse>> ProjectTo => src => new GetCategoryResponse
    {
        Id = src.Id,
        Name = src.Name,
        Description = src.Description,
        Image = src.Image
    };
}