using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using TestRegistrationForWS.Services;

namespace TestRegistrationForWS.Handlers;

public class TokenAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly TokenService _tokenService;

    public TokenAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        TokenService tokenService)
        : base(options, logger, encoder, clock)
    {
        _tokenService = tokenService;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return Task.FromResult(AuthenticateResult.NoResult());

        var token = Request.Headers["Authorization"].ToString();
        if (_tokenService.TokenExists(token))
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "Client") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        return Task.FromResult(AuthenticateResult.Fail("Token not found"));
    }
}