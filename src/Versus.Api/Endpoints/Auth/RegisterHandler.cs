using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Versus.Api.Entities;
using Versus.Api.Extensions;
using Versus.Api.Services.Auth;
using Versus.Shared.Auth;

namespace Versus.Api.Endpoints.Auth;

public class RegisterHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/register", HandleAsync)
        .AllowAnonymous()
        .WithRequestValidation<RegisterRequest>();

    public static async Task<Results<Ok, ValidationProblem>> HandleAsync(
        RegisterRequest request,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var user = new User
        {
            UserName = request.Login,
            Email = request.Login,
            Image = request.Image,
            FirstName = request.FirstName,
            YearOfBirth = request.YearOfBirth,
            Gender = (UserGender)request.Gender,
            LanguageCode = request.Language,
            City = request.City
        };

        try
        {
            await userService.CreateAsync(user, request.Password, cancellationToken);
        }
        catch (ValidationException ex)
        {
            return TypedResults.ValidationProblem(ex.Errors.ToDictionary(x => x.PropertyName, x => new[]
            {
                x.ErrorMessage
            }));
        }

        return TypedResults.Ok();
    }
}