using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Versus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApiControllerBase : ControllerBase
{
}
