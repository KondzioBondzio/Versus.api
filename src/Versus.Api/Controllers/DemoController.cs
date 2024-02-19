using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Versus.Api.Controllers;

public class DemoController : ApiControllerBase
{
    [Authorize]
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Content($"{User.Identity?.IsAuthenticated} | {User.FindFirstValue(ClaimTypes.NameIdentifier)} | {User.FindFirstValue(ClaimTypes.Email)}");
    }
}
