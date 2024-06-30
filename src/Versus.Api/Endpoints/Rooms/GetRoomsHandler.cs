using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Common;
using Versus.Shared.Rooms;

namespace Versus.Api.Endpoints.Rooms;

public class GetRoomsHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapGet("/", HandleAsync)
        .WithRequestValidation<GetRoomsRequest>()
        .Produces<Ok<PagedResponse<GetRoomResponse>>>()
        .Produces<UnauthorizedHttpResult>();

    public static async Task<IResult> HandleAsync(
        [AsParameters] GetRoomsRequest request,
        ClaimsPrincipal claimsPrincipal,
        VersusDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var data = await dbContext.Rooms
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Host)
            .Skip((request.PageNumber - 1) * request.PageSize ?? 0)
            .Take(request.PageSize ?? 10)
            .Select(ProjectTo)
            .ToListAsync(cancellationToken);

        var result = new PagedResponse<GetRoomsResponse>(0, data.Count, data.Count, data);
        return TypedResults.Ok(result);
    }

    private static Expression<Func<Room, GetRoomsResponse>> ProjectTo => src => new GetRoomsResponse
    {
        Id = src.Id,
        Name = src.Name,
        PasswordProtected = !string.IsNullOrEmpty(src.Password),
        PlayDate = src.PlayDate,
        CategoryId = src.CategoryId,
        CategoryImage = src.Category.Image,
        CategoryName = src.Category.Name,
        OwnerId = src.HostId,
        OwnerImage = src.Host.Image,
        OwnerName = src.Host.UserName,
        TeamCount = src.Teams.Count(),
        OccupiedSlots = src.Teams.Select(t => t.Users.Count).Sum(),
        TotalSpots = src.Teams.Count * src.TeamPlayerLimit,
    };
}