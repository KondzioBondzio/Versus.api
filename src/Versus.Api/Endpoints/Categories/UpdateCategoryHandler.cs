using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Extensions;
using Versus.Shared.Categories;

namespace Versus.Api.Endpoints.Categories;

public class UpdateCategoryHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPut("/{id:guid}", HandleAsync)
        .WithRequestValidation<CreateCategoryRequest>()
        .Produces<NoContent>()
        .Produces<NotFound>()
        .Produces<ValidationProblem>()
        .Produces<UnauthorizedHttpResult>()
        .Produces<ForbidHttpResult>();

    public static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryRequest request,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var category = await dbContext.Categories
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (category == null)
        {
            return TypedResults.NotFound();
        }

        category.Name = request.Name;
        category.Description = request.Description;
        category.Image = request.Image;
        await dbContext.SaveChangesAsync(cancellationToken);

        // TODO: Return result?
        return TypedResults.NoContent();
    }
}