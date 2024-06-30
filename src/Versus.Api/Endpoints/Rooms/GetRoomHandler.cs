using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Shared.Rooms;
using Team = Versus.Shared.Rooms.Team;
using User = Versus.Shared.Rooms.User;

namespace Versus.Api.Endpoints.Rooms;

public class GetRoomHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapGet("/{id:guid}", HandleAsync);

    public static async Task<Results<Ok<GetRoomResponse>, NotFound, ForbidHttpResult, UnauthorizedHttpResult>>
        HandleAsync(Guid id,
            ClaimsPrincipal claimsPrincipal,
            VersusDbContext dbContext,
            CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var roomExists = await dbContext.Rooms
            .AsNoTracking()
            .AnyAsync(x => x.Id == id, cancellationToken);
        if (!roomExists)
        {
            return TypedResults.NotFound();
        }
        
        var isRoomMember = await dbContext.Rooms
            .AsNoTracking()
            .Include(x => x.Users)
            .Include(x => x.Teams)
            .ThenInclude(x => x.Users)
            .Where(x => x.Id == id)
            .Where(x => x.HostId == userId
                        || x.Users.Any(ru => ru.UserId == userId)
                        || x.Teams.Any(t => t.Users.Any(tu => tu.UserId == userId)))
            .AnyAsync(cancellationToken);
        if (!isRoomMember)
        {
            return TypedResults.Forbid();
        }
        
        var data = await dbContext.Rooms
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Host)
            .Include(x => x.Users)
            .Include(x => x.Teams)
            .ThenInclude(x => x.Users)
            .Include(x => x.ChatMessages)
            .ThenInclude(x => x.User)
            .Where(x => x.Id == id)
            .Select(ProjectToRoom)
            .SingleAsync(cancellationToken);

        return TypedResults.Ok(data);
    }

    private static Expression<Func<Room, GetRoomResponse>> ProjectToRoom => src => new GetRoomResponse
    {
        Id = src.Id,
        Description = src.Description,
        Name = src.Name,
        PasswordProtected = !string.IsNullOrEmpty(src.Password),
        PlayDate = src.PlayDate,
        CategoryId = src.CategoryId,
        CategoryImage = src.Category.Image,
        CategoryName = src.Category.Name,
        OwnerId = src.HostId,
        OwnerImage = src.Host.Image,
        OwnerName = src.Host.UserName,
        Guests = src.Users.Select(ru => new User
        {
            Id = ru.UserId,
            Name = ru.User.UserName,
            Image = ru.User.Image
        }).ToList(),
        Teams = src.Teams.Select(x => new Team
        {
            Id = x.Id,
            Name = x.Name,
            Members = x.Users.Select(tu => new User
            {
                Id = tu.UserId,
                Name = tu.User.UserName,
                Image = tu.User.Image
            }).ToList()
        }).ToList(),
        ChatMessages = src.ChatMessages.Select(x => new ChatMessage
        {
            Id = x.Id,
            UserId = x.UserId,
            UserName = x.User.UserName,
            UserImage = x.User.Image,
            Timestamp = x.Timestamp,
            Content = x.Content
        }).ToList()
    };
}