using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Categories;

namespace Versus.Api.Endpoints.Categories;

public class CreateCategoryHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/", HandleAsync)
        .WithRequestValidation<CreateCategoryRequest>()
        .Produces<Created<Category>>()
        .Produces<ValidationProblem>()
        .Produces<UnauthorizedHttpResult>()
        .Produces<ForbidHttpResult>();

    public static async Task<IResult> HandleAsync(
        CreateCategoryRequest request,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            Image = request.Image
        };

        await dbContext.Categories.AddAsync(category, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Created<object>($"/categories/{category.Id}", null);
    }
}