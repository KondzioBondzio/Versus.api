using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Common;
using Versus.Shared.Rooms;

namespace Versus.Api.Endpoints.Categories;

public class DeleteCategoryHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapDelete("/{id:guid}", HandleAsync)
        .WithRequestValidation<CreateRoomRequest>();

    public static async Task<Results<NoContent, NotFound, Conflict<ConflictResponse>, UnauthorizedHttpResult>>
        HandleAsync(
            [FromRoute] Guid id,
            ClaimsPrincipal claimsPrincipal,
            VersusDbContext dbContext,
            CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var category = await dbContext.Categories
            .Include(x => x.Rooms)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (category == null)
        {
            return TypedResults.NotFound();
        }

        var conflicts = CheckForConflicts(category);
        if (conflicts.Any())
        {
            var conflictResponse = new ConflictResponse(
                "Resource cannot be deleted as there are unresolved conflicts.",
                conflicts
            );
            return TypedResults.Conflict(conflictResponse);
        }

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return TypedResults.NoContent();
    }

    private static ConflictDetail[] CheckForConflicts(Category category)
    {
        List<ConflictDetail> conflicts = new();

        if (category.Rooms.Count != 0)
        {
            conflicts.AddRange(category.Rooms
                .Select(x => ConflictDetail.FromResource<Room, Guid>(x.Id, "Object is dependent on this resource")));
        }

        return conflicts.ToArray();
    }
}