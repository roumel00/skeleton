using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/test")]
[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("public")]
    public ActionResult<string> PublicEndpoint()
    {
        return Ok("This is a public endpoint - no authentication required");
    }

    [HttpGet("protected")]
    [Authorize]
    public ActionResult<string> ProtectedEndpoint()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        
        return Ok($"This is a protected endpoint. User ID: {userId}, Email: {userEmail}");
    }
}
