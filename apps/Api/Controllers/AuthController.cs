using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Services;

namespace Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Invalid input data",
            });
        }

        var result = await _authService.RegisterAsync(registerDto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Invalid input data",
            });
        }

        var result = await _authService.LoginAsync(loginDto);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpGet("me")]
    public async Task<ActionResult> GetCurrentUser()
    {
        var result = await _authService.GetCurrentUserAsync();

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        // Return the user if authenticated, or null if not authenticated
        // Both cases return 200 OK - this is the key change
        return Ok(new { user = result.User });
    }

    [HttpPost("logout")]
    public ActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken");
        return Ok(new AuthResponseDto
        {
            Success = true,
            Message = "Logged out successfully"
        });
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<AuthResponseDto>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Invalid input data",
            });
        }

        var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<AuthResponseDto>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Invalid input data",
            });
        }

        var result = await _authService.ResetPasswordAsync(resetPasswordDto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}