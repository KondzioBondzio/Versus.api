using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Shared.Common;

namespace Versus.Api.Endpoints.Categories;

public class DeleteCategoryHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapDelete("/{id:guid}", HandleAsync)
        .Produces<NoContent>()
        .Produces<NotFound>()
        .Produces<Conflict<ConflictResponse>>()
        .Produces<UnauthorizedHttpResult>()
        .Produces<ForbidHttpResult>();

    public static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var exists = await dbContext.Categories.AnyAsync(x => x.Id == id, cancellationToken);
        if (!exists)
        {
            return TypedResults.NotFound();
        }

        // TODO: merge into single query?
        var isInUse = await dbContext.Categories.AnyAsync(x => x.Id == id && x.Rooms.Count > 0, cancellationToken);
        if (isInUse)
        {
            // Note: return ids which prevent removal?
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                {
                    nameof(id), [Resources.Categories.InUse]
                }
            });
        }

        var category = new Category
        {
            Id = id
        };
        dbContext.Categories.Attach(category);
        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}