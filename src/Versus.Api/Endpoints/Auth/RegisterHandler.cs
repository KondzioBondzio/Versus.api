using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Versus.Api.Entities;
using Versus.Api.Services.Auth;
using Versus.Shared.Auth;

namespace Versus.Api.Endpoints.Auth;

public record RegisterParameters
{
    public IValidator<RegisterRequest> RequestValidator { get; init; } = default!;
    public RegisterRequest Request { get; init; } = default!;
    public IUserService UserService { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out IValidator<RegisterRequest> requestValidator,
        out RegisterRequest request,
        out IUserService userService,
        out CancellationToken cancellationToken)
    {
        requestValidator = RequestValidator;
        request = Request;
        userService = UserService;
        cancellationToken = CancellationToken;
    }
}

public static class RegisterHandler
{
    public static async Task<Results<Ok, ValidationProblem>> HandleAsync
        ([AsParameters] RegisterParameters parameters)
    {
        var (validator, request, userService, cancellationToken) = parameters;

        var user = new User
        {
            UserName = request.Login, Email = request.Email
        };

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        await userService.CreateAsync(user, request.Password, cancellationToken);

        return TypedResults.Ok();
    }
}