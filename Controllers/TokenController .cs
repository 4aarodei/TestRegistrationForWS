using Microsoft.AspNetCore.Mvc;
using TestRegistrationForWS.Services;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly TokenService _tokenService;

    public TokenController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("create")]
    public IActionResult CreateToken([FromBody] TokenRequest request)
    {
        try
        {
            var token = _tokenService.CreateToken(request.WsName, request.ClientId, request.CityId);
            return Ok(new { token });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}

public class TokenRequest
{
    public string WsName { get; set; }
    public string ClientId { get; set; }
    public string CityId { get; set; }
}