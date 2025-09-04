using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Entities;
using Api.Models;

namespace Api.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> GetCurrentUserAsync();
}

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(UserManager<User> userManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User with this email already exists"
                };
            }

            // Create new user
            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                EmailConfirmed = true // For demo purposes, in production you'd want email confirmation
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Registration failed: {errors}"
                };
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully",
                Token = token,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = $"An error occurred: {ex.Message}"
            };
        }
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isValidPassword)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            // Set JWT token as HTTP-only cookie
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = context.Request.IsHttps,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddHours(24)
                };
                context.Response.Cookies.Append("AuthToken", token, cookieOptions);
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = $"An error occurred: {ex.Message}"
            };
        }
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? "YourSuperSecretKeyHere123!@#");
        var issuer = jwtSettings["Issuer"] ?? "YourApp";
        var audience = jwtSettings["Audience"] ?? "YourAppUsers";
        var expirationHours = int.Parse(jwtSettings["ExpirationHours"] ?? "24");

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            }),
            Expires = DateTime.UtcNow.AddHours(expirationHours),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<AuthResponseDto> GetCurrentUserAsync()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "No HTTP context available"
                };
            }

            // Check if user has a valid JWT token
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                // No valid authentication - this is normal for unauthenticated users
                return new AuthResponseDto
                {
                    Success = true,
                    User = null, // No user when not authenticated
                    Message = "No active session"
                };
            }

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                // This shouldn't happen if IsAuthenticated is true, but handle it gracefully
                return new AuthResponseDto
                {
                    Success = true,
                    User = null,
                    Message = "Invalid token format"
                };
            }

            var user = await _userManager.FindByIdAsync(userIdClaim.Value);
            if (user == null)
            {
                // User ID in token doesn't exist in database
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            return new AuthResponseDto
            {
                Success = true,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = $"An error occurred: {ex.Message}"
            };
        }
    }
}
