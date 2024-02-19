using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Services;
using Versus.Shared.Auth;

namespace Versus.Api.Controllers;

[AllowAnonymous]
public class AuthController : ApiControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly VersusDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthController(ITokenService tokenService,
        VersusDbContext dbContext,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _tokenService = tokenService;
        _dbContext = dbContext;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(x => x.UserName == request.Login, cancellationToken);
        if (user == null)
        {
            return Unauthorized();
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        string accessToken = _tokenService.GenerateAccessToken(user);
        string refreshToken = _tokenService.GenerateRefreshToken(user);

        return Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterRequest>> Register([FromBody] RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            UserName = request.Login,
            Email = request.Email
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_tokenService.IsTokenValid(request.Token))
        {
            return Unauthorized();
        }

        ClaimsPrincipal principal = _tokenService.ReadToken(request.Token);
        string? id = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id.ToString() == id, cancellationToken);
        if (user == null)
        {
            return Unauthorized();
        }

        string accessToken = _tokenService.GenerateAccessToken(user);
        string refreshToken = _tokenService.GenerateRefreshToken(user);

        return Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    [HttpGet("login/{scheme}")]
    public async Task<IActionResult> ExternalLogin(string scheme)
    {
        string? redirectUrl = Url.Action(nameof(ExternalLoginCallback), new { scheme });
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

        return scheme switch
        {
            GoogleDefaults.AuthenticationScheme => Challenge(properties, GoogleDefaults.AuthenticationScheme),
            _ => throw new NotSupportedException()
        };
    }

    [HttpGet("login/{scheme}/callback")]
    public async Task<IActionResult> ExternalLoginCallback(string scheme)
    {
        // Handle external login callback, including creating a local user if necessary
        var auth = await Request.HttpContext.AuthenticateAsync(scheme);
        if (!auth.Succeeded)
        {
            return BadRequest();
        }

        var findClaim = (string type) =>
            auth.Principal.Claims
                .Where(x => x.Type == type)
                .Select(x => x.Value)
                .FirstOrDefault();

        return Content($"{auth.Principal?.Identity?.IsAuthenticated} | {findClaim(ClaimTypes.NameIdentifier)} | {findClaim(ClaimTypes.Email)}");
    }
}
