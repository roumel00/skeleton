using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Api.Entities;
using Api.Models;
using Api.Data;

namespace Api.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> GetCurrentUserAsync();
    Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
}

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public AuthService(
        UserManager<User> userManager, 
        IConfiguration configuration, 
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context,
        IEmailService emailService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _emailService = emailService;
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
                EmailConfirmed = true, // For demo purposes, in production you'd want email confirmation
                IsGoogleUser = false // Regular registration
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

            // Set JWT token as HTTP-only cookie (same as login)
            SetAuthCookie(token);

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

            // Check if this is a Google-only user trying to log in with password
            if (user.IsGoogleUser && string.IsNullOrEmpty(user.PasswordHash))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "This account uses Google sign-in. Please use 'Continue with Google' to log in."
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
            SetAuthCookie(token);

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
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim("IsGoogleUser", user.IsGoogleUser.ToString())
        };

        // Add profile picture URL if available
        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
        {
            claims.Add(new Claim("ProfilePictureUrl", user.ProfilePictureUrl));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
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

            var authCookie = context.Request.Cookies["AuthToken"];
            Console.WriteLine($"Auth cookie present: {!string.IsNullOrEmpty(authCookie)}");
            Console.WriteLine($"User authenticated: {context.User.Identity?.IsAuthenticated}");
            Console.WriteLine($"Request scheme: {context.Request.Scheme}");
            Console.WriteLine($"Request host: {context.Request.Host}");

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
            Console.WriteLine($"GetCurrentUserAsync error: {ex.Message}");
            return new AuthResponseDto
            {
                Success = false,
                Message = $"An error occurred: {ex.Message}"
            };
        }
    }

    public async Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            
            // Always return success to prevent email enumeration attacks
            // Don't reveal whether the email exists or not
            
            if (user != null)
            {
                // Check if this is a Google-only user
                if (user.IsGoogleUser && string.IsNullOrEmpty(user.PasswordHash))
                {
                    // Don't send password reset for Google-only users, but still return success
                    return new AuthResponseDto
                    {
                        Success = true,
                        Message = "If an account with that email exists, a password reset link has been sent."
                    };
                }

                // Clean up any existing unused tokens for this user
                var existingTokens = await _context.PasswordResetTokens
                    .Where(t => t.UserId == user.Id && !t.IsUsed)
                    .ToListAsync();
                
                _context.PasswordResetTokens.RemoveRange(existingTokens);

                // Generate secure random token
                var token = GenerateSecureToken();
                
                // Create password reset token entry
                var resetToken = new PasswordResetToken
                {
                    Token = token,
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30), // 30 minutes expiration
                    IsUsed = false
                };

                _context.PasswordResetTokens.Add(resetToken);
                await _context.SaveChangesAsync();

                // Send email
                var userName = !string.IsNullOrEmpty(user.FirstName) ? user.FirstName : user.Email!;
                await _emailService.SendPasswordResetEmailAsync(user.Email!, token, userName);
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "If an account with that email exists, a password reset link has been sent."
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

    public async Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            // Find the token
            var resetToken = await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == resetPasswordDto.Token && !t.IsUsed);

            if (resetToken == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid or expired reset token."
                };
            }

            // Check if token is expired
            if (resetToken.ExpiresAt < DateTime.UtcNow)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Reset token has expired. Please request a new password reset."
                };
            }

            var user = resetToken.User;
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            // Check if this is a Google-only user
            if (user.IsGoogleUser && string.IsNullOrEmpty(user.PasswordHash))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "This account uses Google sign-in and cannot have a password reset."
                };
            }

            // Remove current password
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Failed to reset password. Please try again."
                };
            }

            // Add new password
            var addPasswordResult = await _userManager.AddPasswordAsync(user, resetPasswordDto.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                var errors = string.Join(", ", addPasswordResult.Errors.Select(e => e.Description));
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Failed to set new password: {errors}"
                };
            }

            // Mark token as used
            resetToken.IsUsed = true;
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
                Message = "Password has been reset successfully. You can now log in with your new password."
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

    private void SetAuthCookie(string token)
    {
        var context = _httpContextAccessor.HttpContext;
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

    private string GenerateSecureToken()
    {
        // Generate a 256-bit (32-byte) random token
        using var rng = RandomNumberGenerator.Create();
        var tokenBytes = new byte[32];
        rng.GetBytes(tokenBytes);
        
        // Convert to URL-safe base64 string
        return Convert.ToBase64String(tokenBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
    }
}