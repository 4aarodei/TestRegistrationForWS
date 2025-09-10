using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TestRegistrationForWS.Controllers;

[Authorize(AuthenticationSchemes = "TokenScheme")]
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("hello")]
    public IActionResult Hello()
    {
        return Ok($"Hello, {User.Identity.Name}! Access granted.");
    }
}


