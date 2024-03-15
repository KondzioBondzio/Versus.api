using Microsoft.AspNetCore.Http.HttpResults;
using Versus.Api.Entities;
using Versus.Api.Exceptions;
using Versus.Api.Services.Auth;
using Versus.Shared.Auth;

namespace Versus.Api.Modules.Auth;

public record RegisterParameters
{
    public RegisterRequest Request { get; init; } = default!;
    public IUserService UserService { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out RegisterRequest request,
        out IUserService userService,
        out CancellationToken cancellationToken)
    {
        request = Request;
        userService = UserService;
        cancellationToken = CancellationToken;
    }
}

public static class RegisterHandler
{
    public static async Task<Results<Ok, ProblemHttpResult>> HandleAsync
        ([AsParameters] RegisterParameters parameters)
    {
        var (request, userService, cancellationToken) = parameters;

        var user = new User
        {
            UserName = request.Login,
            Email = request.Email
        };

        try
        {
            await userService.CreateAsync(user, request.Password, cancellationToken);
        }
        catch (ValidationException ex)
        {
            return TypedResults.Problem(
                title: ex.Message,
                detail: string.Join(Environment.NewLine, ex.ValidationErrors),
                statusCode: StatusCodes.Status400BadRequest);
        }

        return TypedResults.Ok();
    }
}
