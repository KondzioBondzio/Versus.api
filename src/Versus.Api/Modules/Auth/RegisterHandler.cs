using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Versus.Api.Entities;
using Versus.Api.Exceptions;
using Versus.Api.Services.Auth;
using Versus.Shared.Auth;

namespace Versus.Api.Modules.Auth;

public static class RegisterHandler
{
    public static async Task<Results<Ok, ProblemHttpResult>> Handle
        ([FromServices] IServiceProvider sp, RegisterRequest request, CancellationToken cancellationToken)
    {
        var userService = sp.GetRequiredService<IUserService>();

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
