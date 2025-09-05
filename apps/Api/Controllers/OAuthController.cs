using Microsoft.AspNetCore.Mvc;
using Api.Services;

namespace Api.Controllers;

[Route("api/oauth")]
[ApiController]
public class OAuthController : ControllerBase
{
    private readonly IGoogleOAuthService _googleOAuthService;

    public OAuthController(IGoogleOAuthService googleOAuthService)
    {
        _googleOAuthService = googleOAuthService;
    }

    [HttpGet("google/start")]
    public ActionResult GoogleStart()
    {
        var authUrl = _googleOAuthService.GetAuthorizationUrl();
        return Redirect(authUrl);
    }

    [HttpGet("google/callback")]
    public async Task<ActionResult> GoogleCallback([FromQuery] string? code, [FromQuery] string? error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            var frontendUrl = HttpContext.RequestServices
                .GetRequiredService<IConfiguration>()["Frontend:BaseUrl"] ?? "http://localhost:3000";
            return Redirect($"{frontendUrl}/login?error=oauth_error");
        }

        if (string.IsNullOrEmpty(code))
        {
            var frontendUrl = HttpContext.RequestServices
                .GetRequiredService<IConfiguration>()["Frontend:BaseUrl"] ?? "http://localhost:3000";
            return Redirect($"{frontendUrl}/login?error=no_code");
        }

        var result = await _googleOAuthService.HandleCallbackAsync(code);

        var frontendBaseUrl = HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()["Frontend:BaseUrl"] ?? "http://localhost:3000";

        if (!result.Success)
        {
            return Redirect($"{frontendBaseUrl}/login?error=oauth_failed");
        }

        // Set JWT cookie (same logic as regular login)
        SetAuthCookie(result.Token!);

        return Redirect($"{frontendBaseUrl}/dashboard");
    }

    private void SetAuthCookie(string token)
    {
        var context = HttpContext;
        if (context == null) return;

        // Check if this is a cross-origin request
        var isHttps = context.Request.IsHttps;
        var origin = context.Request.Headers["Origin"].FirstOrDefault();
        var isCrossOrigin = !string.IsNullOrEmpty(origin) && 
                           !origin.Contains(context.Request.Host.Host);

        // For cross-origin HTTPS requests, we need SameSite=None and Secure=true
        var useSameSiteNone = isHttps && isCrossOrigin;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = useSameSiteNone, // Must be true for SameSite=None
            SameSite = useSameSiteNone ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(24),
            Path = "/"
        };

        context.Response.Cookies.Append("AuthToken", token, cookieOptions);
    }
}