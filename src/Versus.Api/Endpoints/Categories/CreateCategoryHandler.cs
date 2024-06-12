using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Categories;

namespace Versus.Api.Endpoints.Categories;

public class CreateCategoryHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/", HandleAsync)
        .WithRequestValidation<CreateCategoryRequest>();

    public static async Task<Results<Created, Conflict, UnauthorizedHttpResult>> HandleAsync(
        CreateCategoryRequest request,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var exists = await dbContext.Categories
            .AsNoTracking()
            .AnyAsync(x => x.Name == request.Name, cancellationToken);
        if (exists)
        {
            return TypedResults.Conflict();
        }

        var category = new Category
        {
            Name = request.Name, Description = request.Description, Image = request.Image
        };

        await dbContext.Categories.AddAsync(category, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Created();
    }
}