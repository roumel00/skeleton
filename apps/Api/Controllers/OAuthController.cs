using Microsoft.AspNetCore.Mvc;
using Api.Services;

namespace Api.Controllers;

[Route("api/oauth")]
[ApiController]
public class OAuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IConfiguration _configuration;

    public OAuthController(IAuthenticationService authenticationService, IConfiguration configuration)
    {
        _authenticationService = authenticationService;
        _configuration = configuration;
    }

    [HttpGet("google/start")]
    public ActionResult GoogleStart()
    {
        var authUrl = _authenticationService.GetOAuthAuthorizationUrl("google");
        return Redirect(authUrl);
    }

    [HttpGet("google/callback")]
    public async Task<ActionResult> GoogleCallback(
        [FromQuery] string? code, 
        [FromQuery] string? state, 
        [FromQuery] string? error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";
            return Redirect($"{frontendUrl}/login?error=oauth_error");
        }

        if (string.IsNullOrEmpty(code))
        {
            var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";
            return Redirect($"{frontendUrl}/login?error=no_code");
        }

        if (string.IsNullOrEmpty(state))
        {
            var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";
            return Redirect($"{frontendUrl}/login?error=invalid_state");
        }

        var result = await _authenticationService.HandleOAuthCallbackAsync("google", code, state);

        var frontendBaseUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";

        if (!result.Success)
        {
            return Redirect($"{frontendBaseUrl}/login?error=oauth_failed");
        }

        // Cookie is already set by the AuthenticationService
        return Redirect($"{frontendBaseUrl}/dashboard");
    }
}